using BakedRaspberryPi.Models;
using System;
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
                    UPC = "XXXX",
                    Name = "NONE",
                    Model = "NONE",
                    Description = "NO PI CHOSEN",
                    Price = 0m,
                    Image = ""
                });

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
        public ActionResult Index(int? thePiBoardId)
        {
            Guid cartId;
            Cart c = null;
            int WholePiToBeEdited = 0;
            decimal priceToBeDeducted = 0m;

            

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
            else
            {
                foreach (var wholePi in c.WholePis)
                {
                    if (wholePi.EditPreviousId != 0)
                    {
                        WholePiToBeEdited = wholePi.WholePiId;
                    }
                }
            }

            WholePi currentPi;

            if (WholePiToBeEdited != 0)
            {
                currentPi = c.WholePis.FirstOrDefault(x => x.WholePiId == WholePiToBeEdited);
            }
            else if (c.CurrentPiId == 0)
            {
                currentPi = c.WholePis.FirstOrDefault();
            }
            else
            {
                currentPi = c.WholePis.First(x => x.WholePiId == c.CurrentPiId);
            }

            if (!(Object.ReferenceEquals(null, currentPi)) && currentPi.Pi != null && currentPi.Pi.Price != 0m)
            {
                priceToBeDeducted = currentPi.Pi.Price;
            }

            if (currentPi == null)
            {
                currentPi = new WholePi();
                currentPi.Quantity = 1;
                c.WholePis.Add(currentPi);
            }

            if (thePiBoardId == null)
            {
                currentPi.Pi = db.Pis.Find(1);
            }
            else
            {
                currentPi.Pi = db.Pis.Find(thePiBoardId);
            }

            currentPi.Price -= priceToBeDeducted;
            currentPi.Price += currentPi.Pi.Price;
            db.SaveChanges();
            
            if (currentPi.IsEdit == true)
            {               
                currentPi.IsEdit = false;
                currentPi.EditPreviousId = 0;
                currentPi.Pi.IsEdit = false;
                currentPi.Pi.EditPreviousId = 0;
                db.SaveChanges();
                return RedirectToAction("Index", "Cart");
            }

            if (thePiBoardId == null)
            {
                return RedirectToAction("Index", "Accessory");
            }

            return RedirectToAction("Index", "OS");
        }

        public ActionResult Edit(WholePi WholePiPi, int wholePiToBeEditedId, int previousPiBoard)
        {
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            toEdit.EditPreviousId = wholePiToBeEditedId;
            toEdit.Pi.IsEdit = true;
            toEdit.Pi.EditPreviousId = previousPiBoard;
            db.SaveChanges();
            return RedirectToAction("Index", "Pi");
        }
    }
}
