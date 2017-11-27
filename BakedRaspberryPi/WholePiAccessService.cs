using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi
{
    public class WholePiAccessService : Controller
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
        /// <summary>
        /// This function finds a whole Pi in a database and returns that database.
        /// </summary>
        public WholePi findWholePi()
        {
            WholePi WholePiToReturn = new WholePi();


            Guid cartId;
            Cart c = null;

            // if there's a cookie of the cartId, use the cart in the db with that cartId
            if (Request.Cookies.AllKeys.Contains("cartId"))
            {
                cartId = Guid.Parse(Request.Cookies["cartId"].Value);
                c = db.Carts.Find(cartId);
            }

            // There must not be a cookie of the cart, make a new cart
            if (c == null)
            {
                c = new Cart();
                cartId = Guid.NewGuid();
                c.CartId = cartId;
                db.Carts.Add(c);
                db.SaveChanges();
                Response.Cookies.Add(new HttpCookie("cartId", cartId.ToString()));
            }

            if (c.WholePis == null)
            {
                c.WholePis = new List<WholePi>();
            }
            WholePi currentPi = c.WholePis.FirstOrDefault();
            if (currentPi == null)
            {
                currentPi = new WholePi();
                c.WholePis.Add(currentPi);
            }




            return WholePiToReturn;
        }
    }
}