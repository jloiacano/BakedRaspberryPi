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
            Models.CheckoutDetails details = new Models.CheckoutDetails();
            Guid cartId = Guid.Parse(Request.Cookies["cartId"].Value);

            details.CurrentCart = db.Carts.Find(cartId);

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Index(Models.CheckoutDetails model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Persist this order to the database
                //TODO: send some confirmation emails to the person placing the order and the system admin
                //TODO: Reset the cart
                return RedirectToAction("Index", "Receipt");
            }
            return View(model);
        }
    }
}