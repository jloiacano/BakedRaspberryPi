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
            if (!db.Carts.Any())
            {
                return RedirectToAction("Index", "Home");
            }
            // gets the cartId from cookies
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            // gets the cart with the cartId found in cookies from the database, and sends it to the 'cart view'
            return View(db.Carts.Find(cartID));
        }

        // POST: Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.Cart sentCart)
        {

            //var cart = db.Carts.Find(model.ID);
            //for (int i = 0; i < model.CartProducts.Count; i++)
            //{
            //    cart.CartProducts.ElementAt(i).Quantity = model.CartProducts.ElementAt(i).Quantity;
            //}
            //db.SaveChanges();

            var theDBCart = db.Carts.Find(sentCart.CartId);

            foreach (var aWholPi in sentCart.WholePis)
            {
                Console.WriteLine();
            }
            
            for (int i = 0; i < sentCart.WholePis.Count; i++)
            {
                var theCartsWholePi = sentCart.WholePis.ElementAt(i);
                var theDBsWholePi = theDBCart.WholePis.Where(x => x.WholePiId == theCartsWholePi.WholePiId);
            }
            db.SaveChanges();
            return View(sentCart);
        }
        
        public ActionResult Remove(int toBeRemoved, Guid theCartId)
        {
            Cart theCart = db.Carts.Find(theCartId);
            theCart.WholePis.Remove(theCart.WholePis.FirstOrDefault(x => x.WholePiId == toBeRemoved));
            db.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult UpdateQuantities(int howMany, int wholePiToBeUpdated, Guid theCartId)
        {
            var theCart = db.Carts.Find(theCartId);
            theCart.WholePis.ElementAt(wholePiToBeUpdated).Quantity = howMany;
            db.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult AddAnotherWholePi(Guid theCartId)
        {
            Cart theCart = db.Carts.Find(theCartId);
            WholePi aNewWholePi = new WholePi();
            theCart.WholePis.Add(aNewWholePi);
            if (theCart.CurrentPiId == 0)
            {
                theCart.CurrentPiId = 2;
            }
            else
            {
                theCart.CurrentPiId += 1;
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Pi");
        }
    }
}