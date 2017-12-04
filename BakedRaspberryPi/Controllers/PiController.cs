using BakedRaspberryPi.Models;
using RestSharp;
using RestSharp.Authenticators;
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
            }

            db.SaveChanges();
            return View(db.Pis.ToList());
        }

        [HttpPost]
        public ActionResult Index(int? thePiBoardId)
        {
            Guid cartId;
            Cart c = null;
            int WholePiToBeEdited = 0;
            decimal priceToBeDeducted = 0m;
            string[] piNames = new string[] {
                "Very Raspberry Pi",
                "Lemon-Raspberry Pi",
                "Raspberry Custard Pi",
                "Apple-Raspberry Pi",
                "Nectarine-Raspberry Pi",
                "Pear-Raspberry Pi",
                "Low-Fat Raspberry Pi",
                "Gluten Free Raspberry Pi",
                "Organic Raspberry Pi",
                "Sugar-Free Raspberry Pi",
                "Figgy Raspberry Pi",
                "Vegan Raspberry Pi",
                "Amish Raspberry Custard Pi",
                "Rhubarb-Raspberry Pi",
                "Peach Raspberry Pi",
                "Key-Lime and Raspberry Pi",
                "Raspberry-Blueberry Pi"
            };

            

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
                Response.Cookies.Add(new System.Web.HttpCookie("cartId", cartId.ToString()));
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
                c.WholePis.Add(currentPi);
            }

            if (currentPi.Quantity == 0)
            {
                currentPi.Quantity = 1;
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
            //testSendSimpleMessage();
            
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
                currentPi.Filling = db.OSs.Find(1);
                db.SaveChanges();
                return RedirectToAction("Index", "Accessory");
            }

            return RedirectToAction("Index", "OS");
        }

        public ActionResult Edit(WholePi WholePiPi, int wholePiToBeEditedId, int previous)
        {
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            toEdit.EditPreviousId = wholePiToBeEditedId;
            toEdit.Pi.IsEdit = true;
            toEdit.Pi.EditPreviousId = previous;
            db.SaveChanges();
            return RedirectToAction("Index", "Pi");
        }

        public static RestResponse testSendSimpleMessage()
        {
            string MailGunApiKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.ApiKey"];
            string MailGunPubKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PubKey"];
            string MailGunSandBox = System.Configuration.ConfigurationManager.AppSettings["Mailgun.SandBox"];
            string MailGunPostMan = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PostMan"];
            string MailGunTestRecipient = System.Configuration.ConfigurationManager.AppSettings["Mailgun.TestRecipient"];

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", MailGunApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", MailGunSandBox, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@" + MailGunSandBox + ">");
            request.AddParameter("to", "RecipientName <" + MailGunTestRecipient + ">");
            request.AddParameter("subject", "Testing MailGun");
            request.AddParameter("text", "Testing MailGun");
            request.Method = Method.POST;
            RestResponse restResponse = (RestResponse)client.Execute(request);
            return restResponse;
            throw new NotImplementedException();
        }
    }
}
