using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class CheckoutController : Controller
    {
        BakedPiModels db = new BakedPiModels();

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
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            details.CurrentCart = db.Carts.Find(cartID);

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Index(CheckoutDetails model)
        {


            //model.CurrentCart = Models.Cart.BuildCart(Request);
            int cartID = Int32.Parse(Request.Cookies["cartID"].Value);

            model.CurrentCart = db.Carts.Find(cartID);

            //if (ModelState.IsValid)
            //{
            //    string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8);
            //    db.Orders.Add(new Order
            //    {
            //        DateCreated = DateTime.UtcNow,
            //        DateLastModified = DateTime.UtcNow,
            //        TrackingNumber = trackingNumber,
            //        ShippingAndHandling = model.CurrentCart.CartProducts.Sum(x => x.Quantity),
            //        Tax = (model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0) * .1025m,
            //        SubTotal = model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0,
            //        Email = model.ContactEmail,
            //        PurchaserName = model.ContactName,s
            //        ShippingAddress1 = model.ShippingAddress,
            //        ShippingCity = model.ShippingCity,
            //        ShippingPostalCode = model.ShippingPostalCode,
            //        ShippingState = model.ShippingState
            //    });

            //    db.SaveChanges();

            //    string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            //    string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            //    string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            //    string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];

            //    Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
            //    transaction.Amount = 1m;
            //    //transaction.Amount = o.SubTotal + o.ShippingAndHandling + o.Tax;
            //    //transaction.TaxAmount = o.Tax;
            //    //https://developers.braintreepayments.com/reference/general/testing/ruby
            //    transaction.CreditCard = new Braintree.TransactionCreditCardRequest
            //    {
            //        CardholderName = "Test User",
            //        CVV = "123",
            //        Number = "4111111111111111",
            //        ExpirationYear = DateTime.Now.AddMonths(1).Year.ToString(),
            //        ExpirationMonth = DateTime.Now.AddMonths(1).ToString("MM")
            //    };

            //    var result = gateway.Transaction.Sale(transaction);

            //    BakedPiEmailService emailService = new BakedPiEmailService();
            //    emailService.SendAsync(new Microsoft.AspNet.Identity.IdentityMessage
            //    {
            //        Subject = "Your Receipt for order " + trackingNumber,
            //        Destination = model.ContactEmail,
            //        Body = "Thank you for shopping."
            //    });

            //    //TODO: Reset the cart
            //    return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
            //}
            return View(model);
        }
    }
}