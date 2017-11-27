using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class CartController : Controller
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

        // GET: Cart
        public ActionResult Index()
        {
            // gets the cartId from cookies
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            // gets the cart with the cartId found in cookies from the database, and sends it to the 'cart view'
            return View(db.Carts.Find(cartID));
        }

        // POST: Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Cart model)
        {
            var cart = db.Carts.Find(model.CartId);
            for (int i = 0; i < model.WholePis.Count; i++)
            {
                //Change this to fit my model
                decimal Total = cart.WholePis.Sum(x => x.Price);
                //cart.CartProducts.ElementAt(i).Quantity = model.CartProducts.ElementAt(i).Quantity;
            }
            db.SaveChanges();
            return View(cart);
        }

        
    }
}