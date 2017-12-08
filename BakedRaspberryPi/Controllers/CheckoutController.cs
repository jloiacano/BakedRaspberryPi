using BakedRaspberryPi.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using Braintree;

namespace BakedRaspberryPi.Controllers
{
    public class CheckoutController : Controller
    {
        protected BakedPiModels db = new BakedPiModels();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Checkout
        public ActionResult Index()
        {
            CheckoutDetails details = new CheckoutDetails();
            Guid cartId = Guid.Parse(Request.Cookies["cartId"].Value);

            details.CurrentCart = db.Carts.Find(cartId);

            details.Addresses = new Braintree.Address[0];

            if (User.Identity.IsAuthenticated)
            {
                string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
                string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
                string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
                string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                Braintree.BraintreeGateway gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

                var customerGateway = gateway.Customer;

                Braintree.CustomerSearchRequest query = new Braintree.CustomerSearchRequest();
                query.Email.Is(User.Identity.Name);
                var matchedCustomers = customerGateway.Search(query);
                Braintree.Customer customer = null;
                if (matchedCustomers.Ids.Count == 0)
                {
                    Braintree.CustomerRequest newCustomer = new Braintree.CustomerRequest();
                    newCustomer.Email = User.Identity.Name;

                    var result = customerGateway.Create(newCustomer);
                    customer = result.Target;
                }
                else
                {
                    customer = matchedCustomers.FirstItem;
                }

                details.Addresses = customer.Addresses;
            }

            return View(details);

        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.CheckoutDetails model, string addressId)
        {

            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            model.CurrentCart = db.Carts.Find(cartID);
            string orderCreditCardType = "";
            string maskedCC = "";

            string formattedCCNumber = Regex.Replace(model.CreditCardNumber, @"[^0-9]", "");

            if (ModelState.IsValid)
            {
                string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                decimal tax = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity) * .1025m;
                decimal subtotal = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity);
                decimal shipping = model.CurrentCart.WholePis.Sum(x => x.Quantity);
                decimal total = subtotal + tax + shipping;

                BakedPiPaymentServices payments = new BakedPiPaymentServices();
                string email = User.Identity.IsAuthenticated ? User.Identity.Name : model.ContactEmail;
                Result<Transaction> authorizeReply = payments.AuthorizeCard(email, total, tax, trackingNumber, addressId, model.CardholderName, model.CVV, formattedCCNumber, model.ExpirationMonth, model.ExpirationYear);

                if (string.IsNullOrEmpty(authorizeReply.Message))
                {
                    Order order = new Order
                    {
                        TrackingNumber = trackingNumber,
                        MaskedCC = authorizeReply.Target.CreditCard.MaskedNumber,
                        CCImage = authorizeReply.Target.CreditCard.ImageUrl,
                        CCType = authorizeReply.Target.CreditCard.CardType.ToString().ToUpper(),
                        Email = model.ContactEmail,
                        PurchaserName = model.ContactName,
                        ShippingAddress1 = model.ShippingAddress,
                        ShippingCity = model.ShippingCity,
                        ShippingState = model.ShippingState,
                        ShippingPostalCode = model.ShippingPostalCode,
                        SubTotal = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity),
                        ShippingAndHandling = (model.CurrentCart.WholePis.Sum(x => x.Quantity) * 1.5m),
                        Tax = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity) * .1025m,
                        DateCreated = DateTime.UtcNow,
                        DateLastModified = DateTime.UtcNow
                    };

                    db.Orders.Add(order);

                    db.SaveChanges();
                    
                    EmailMessageMaker receiptMailMessage = new EmailMessageMaker();
                    receiptMailMessage.Line.Add("<h2>The Pi is now being baked...</h2>");
                    receiptMailMessage.Line.Add("<h4>Thank you for your Order!</h4>");
                    receiptMailMessage.Line.Add("Order# " + order.TrackingNumber);
                    receiptMailMessage.Line.Add("<h2>Shipping To:</h2>");
                    receiptMailMessage.Line.Add(order.PurchaserName);
                    receiptMailMessage.Line.Add(order.ShippingAddress1);
                    receiptMailMessage.Line.Add(order.ShippingCity + ", " + order.ShippingState + " " + order.ShippingPostalCode);
                    receiptMailMessage.Line.Add("<br /><br />");
                    receiptMailMessage.Line.Add("Total: " + (order.SubTotal + order.ShippingAndHandling + order.Tax).ToString("C"));
                    receiptMailMessage.Line.Add("<br />Paid by: " + order.CCType + " " + order.MaskedCC);
                    receiptMailMessage.Line.Add("<br />");
                    receiptMailMessage.Line.Add("Thanks again for shopping with us today! Enjoy your Pi!");
                    string receiptSubject = "Thank you for your order! (Order: " + order.TrackingNumber + ")";
                    string receiptRecipient = order.Email;
                    
                    //Mail the Receipt
                    PiMailer receiptMail = new PiMailer(receiptRecipient, receiptSubject, receiptMailMessage);
                    receiptMail.SendMail();

                    //Reset the cart - Trash the cookie, so they'll get a new cart next time they need one
                    Response.SetCookie(new System.Web.HttpCookie("cartID") { Expires = DateTime.UtcNow });
                    //If you have a cart table, you can clear this cart out since it has now been converted to an order
                    db.WholePis.RemoveRange(model.CurrentCart.WholePis);
                    db.Carts.Remove(model.CurrentCart);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
                }

                ModelState.AddModelError("CreditCardNumber", authorizeReply.Message);
            }
            return View(model);                
        }
    }
}