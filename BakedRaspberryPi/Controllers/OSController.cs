﻿using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            
            if (!db.Carts.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!db.OSs.Any())
            {
                List<OS> oss = new List<OS>();

                oss.Add(new OS
                {
                    Name = "NONE",
                    Description = "",
                    Image = "",
                    Price = 0m
                });

                oss.Add(new OS
                {
                    Name = "OSMC",
                    Description = "The best Home Theater PC Option",
                    Image = "/Images/OSs/osmc.png",
                    Price = 5m
                });

                oss.Add(new OS
                {
                    Name = "Debian",
                    Description = "The FreeBSD works on Pi too!",
                    Image = "/Images/OSs/debian.jpg",
                    Price = 5m
                });

                oss.Add(new OS
                {
                    Name = "Raspbian",
                    Description = "Debian, but specifically ported to the Pi",
                    Image = "/Images/OSs/raspbian.jpg",
                    Price = 5m
                });

                oss.Add(new OS
                {
                    Name = "Arch Linux ARM",
                    Description = "Linux, ported ARM systems",
                    Image = "/Images/OSs/archlinux.png",
                    Price = 5m
                });

                oss.Add(new OS
                {
                    Name = "Ubuntu",
                    Description = "The most powerfull OS for the Pi (resource intensive)",
                    Image = "/Images/OSs/ubuntu.svg",
                    Price = 5m
                });

                db.OSs.AddRange(oss);
                db.SaveChanges();
            }

            return View(db.OSs.ToList());
        }

        [HttpPost]
        public ActionResult Index(int? value)
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

            // Set a list of wholePis or find which wholepi is being edited
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

            // Get the price to be deducted during the edit
            if (!(Object.ReferenceEquals(null, currentPi)) && currentPi.Filling != null && currentPi.Filling.Price != 0m)
            {
                priceToBeDeducted = currentPi.Filling.Price;
            }

            // Set currentPi to a blank WholePi
            if (currentPi == null)
            {
                currentPi = new WholePi();
                c.WholePis.Add(currentPi);
            }

            // If no OS was chosen, set it to 1 (null), or find the chosen OS and set it
            if (value == null)
            {
                currentPi.Filling = db.OSs.Find(1);
            }
            else
            {
            currentPi.Filling = db.OSs.Find(value);
            }

            currentPi.Price -= priceToBeDeducted;
            currentPi.Price += currentPi.Filling.Price;
            db.SaveChanges();

            if (currentPi.IsEdit == true)
            {
                currentPi.IsEdit = false;
                currentPi.EditPreviousId = 0;
                currentPi.Filling.IsEdit = false;
                currentPi.Filling.EditPreviousId = 0;
                db.SaveChanges();
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Index", "Accessory");
        }

        public ActionResult Edit(WholePi WholePiPi, int wholePiToBeEditedId, int previousOs)
        {
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            toEdit.Filling.IsEdit = true;
            toEdit.Filling.EditPreviousId = previousOs;
            db.SaveChanges();
            return RedirectToAction("Index", "OS");
        }
    }
}
