using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BakedRaspberryPi.Models;

namespace BakedRaspberryPi.Controllers
{
    public class OSController : Controller
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

        // GET: OSs
        public ActionResult Index()
        {

            if (!db.OSs.Any())
            {

                List<OS> oss = new List<OS>();

                oss.Add(new OS
                {
                    Name = "OSMC",
                    Description = "The best Home Theater PC Option",
                    Image = "/Images/OSs/osmc.png"
                });

                oss.Add(new OS
                {
                    Name = "Debian",
                    Description = "The FreeBSD works on Pi too!",
                    Image = "/Images/OSs/debian.jpg"
                });

                oss.Add(new OS
                {
                    Name = "Raspbian",
                    Description = "Debian, but specifically ported to the Pi",
                    Image = "/Images/OSs/raspbian.jpg"
                });

                oss.Add(new OS
                {
                    Name = "Arch Linux ARM",
                    Description = "Linux, ported ARM systems",
                    Image = "/Images/OSs/archlinux.png"
                });

                oss.Add(new OS
                {
                    Name = "Ubuntu",
                    Description = "The most powerfull OS for the Pi (resource intensive)",
                    Image = "/Images/OSs/ubuntu.svg"
                });

                db.OSs.AddRange(oss);
                db.SaveChanges();
            }

            return View(db.OSs.ToList());
        }

        [HttpPost]
        public ActionResult Index(int id)
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
                cartId = c.Id; //shouldnt this get the cartId from the database id of the cart instead of the newed up cart in the app?
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
            currentPi.Filling = db.OSs.Find(id);
            db.SaveChanges();

            if (currentPi.Pi == null)
            {
                return RedirectToAction("Index", "Pi");
            }

            if (currentPi.Crust == null)
            {
                return RedirectToAction("Index", "Accessories");
            }

            return RedirectToAction("Index", "Cart");

        }
    }
}