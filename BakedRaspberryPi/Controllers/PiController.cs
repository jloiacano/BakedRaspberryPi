using BakedRaspberryPi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class PiController : Controller
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

        public ActionResult Index()
        {
            //var results = db.Pis.ToArray();
            if (!db.Pis.Any())
            {
                List<Pi> pis = new List<Pi>();

                pis.Add(new Pi
                {
                    UPC = "742741039962",
                    Name = "Pi Zero",
                    Model = "Raspberry Pi Zero - Version 1.3",
                    Description = "Tiny and Powerful. The applications of this product are endless!",
                    Price = 5.0m,
                    Image = "/Images/Pi/piZero.jpg"
                });

                pis.Add(new Pi
                {
                    UPC = "640522710515",
                    Name = "Pi2",
                    Model = "Raspberry Pi 2 Model B 1gb Ram Quad Core 900mhz Cpu 2015 Edition",
                    Description = "Oldie but a goodie. Storng, consistent operation. They don't make 'em like they used to!",
                    Price = 38.0m,
                    Image = "/Images/Pi/pi2.jpg"
                });

                pis.Add(new Pi
                {
                    UPC = "640522710850",
                    Name = "Pi3",
                    Model = "Raspberry Pi 3 Model B - 1.2GHz Quad Core Cortex-A53 64-Bit CPU, 1GB RAM, WiFi",
                    Description = "Newest model. More memory, and faster processor, with WiFi and BlueTooth built in!",
                    Price = 36.0m,
                    Image = "/Images/Pi/pi3.jpg"
                });
                db.Pis.AddRange(pis);
                db.SaveChanges();
            }

            return View(db.Pis.ToList());
        }

        [HttpPost]
        public ActionResult Index(int value)
        {
            int cartId;
            Cart c = null;

            // if there's a cookie of the cartId, use the cart in the db with that cartId
            if (Request.Cookies.AllKeys.Contains("cartId"))
            {
                cartId = int.Parse(Request.Cookies["cartId"].Value);
                c = db.Carts.Find(cartId);
            }

            // There must not be a cookie of the cart, make a new cart
            if (c == null)
            {
                c = new Cart();
                db.Carts.Add(c);
                db.SaveChanges();
                cartId = c.CartId;
                Response.Cookies.Add(new HttpCookie("cartId", cartId.ToString()));
            }

            if (c.CurrentPis == null)
            {
                c.CurrentPis = new List<WholePi>();
            }
            WholePi currentPi = c.CurrentPis.FirstOrDefault();
            if (currentPi == null)
            {
                currentPi = new WholePi();
                c.CurrentPis.Add(currentPi);
            }
            currentPi.Pi = db.Pis.Find(value);
            db.SaveChanges();

            if (currentPi.Filling == null)
            {
                return RedirectToAction("Index", "OS");
            }

            if (currentPi.Crust == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}