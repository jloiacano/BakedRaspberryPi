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

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.CheckoutDetails model)
        {

            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            model.CurrentCart = db.Carts.Find(cartID);
            string orderCreditCardType ="";
            string maskedCC = "";

            if (ModelState.IsValid)
            {
                string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8);
                if (model.CreditCardNumber.Length == 16)
                {
                    maskedCC = "#### #### #### " + model.CreditCardNumber.Substring(12, 4);
                    if (int.Parse(model.CreditCardNumber.Substring(0, 1)) == 4)
                    {
                        orderCreditCardType = "VISA";
                    }
                    else if ( (510000 < int.Parse(model.CreditCardNumber.Substring(0, 6)) && 559999 > int.Parse(model.CreditCardNumber.Substring(0, 6))) ||
                        (222100 < int.Parse(model.CreditCardNumber.Substring(0, 6)) && 272099 > int.Parse(model.CreditCardNumber.Substring(0, 6))) )
                    {
                        orderCreditCardType = "MASTERCARD";
                    } else
                    {
                        orderCreditCardType = "Unknown";
                    }

                }
                else if (model.CreditCardNumber.Length == 15)
                {
                    maskedCC = "#### ###### " + model.CreditCardNumber.Substring(10, 5);
                    orderCreditCardType = "AMEX";
                }

                Order order = new Order
                {
                    TrackingNumber = trackingNumber,
                    MaskedCC = maskedCC,
                    CCType = orderCreditCardType,
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

                var orderTotal = order.SubTotal + order.ShippingAndHandling + order.Tax;

                EmailMessageMaker receiptMailMessage = new EmailMessageMaker();
                receiptMailMessage.Line.Add("<h2>The Pi is now being baked...</h2>");
                receiptMailMessage.Line.Add("<h4>Thank you for your Order!</h4>");
                receiptMailMessage.Line.Add("Order# " + order.TrackingNumber);
                receiptMailMessage.Line.Add("<h2>Shipping To:</h2>");
                receiptMailMessage.Line.Add(order.PurchaserName);
                receiptMailMessage.Line.Add(order.ShippingAddress1);
                receiptMailMessage.Line.Add(order.ShippingCity + ", " + order.ShippingState + " " + order.ShippingPostalCode);
                receiptMailMessage.Line.Add("<br /><br />");
                receiptMailMessage.Line.Add("Total: " + orderTotal.ToString("C"));
                receiptMailMessage.Line.Add("<br />Paid by: " + order.CCType + " " + order.MaskedCC);
                receiptMailMessage.Line.Add("<br />");
                receiptMailMessage.Line.Add("Thanks again for shopping with us today! Enjoy your Pi!");
                string receiptSubject = "Thank you for your order! (Order: " + order.TrackingNumber + ")";
                string receiptRecipient = order.Email;

                db.Orders.Add(order);

                db.SaveChanges();

                string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
                string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
                string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
                string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                Braintree.BraintreeGateway gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest
                {
                    Amount = order.SubTotal + order.ShippingAndHandling + order.Tax,
                    TaxAmount = order.Tax,
                    OrderId = trackingNumber,
                    CreditCard = new Braintree.TransactionCreditCardRequest
                    {
                        CardholderName = model.CardholderName,
                        CVV = model.CVV,
                        Number = model.CreditCardNumber,
                        ExpirationYear = model.ExpirationYear,
                        ExpirationMonth = model.ExpirationMonth
                    }
                };

                var result = gateway.Transaction.Sale(transaction);

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

            return View();                
        }
    }
}