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

            if (ModelState.IsValid)
            {
                string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8);

                Order o = new Order();

                o.TrackingNumber = trackingNumber;
                o.Email = model.ContactEmail;
                o.PurchaserName = model.ContactName;
                o.ShippingAddress1 = model.ShippingAddress;
                o.ShippingCity = model.ShippingCity;
                o.ShippingState = model.ShippingState;
                o.ShippingPostalCode = model.ShippingPostalCode;
                o.SubTotal = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity);
                o.ShippingAndHandling = (model.CurrentCart.WholePis.Sum(x => x.Quantity) * 1.5m);
                o.Tax = model.CurrentCart.WholePis.Sum(x => x.Price * x.Quantity) * .1025m;
                o.DateCreated = DateTime.UtcNow;
                o.DateLastModified = DateTime.UtcNow;
                

                db.Orders.Add(o);

                db.SaveChanges();
                return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
            }

            return View();                
        }
    }
}