using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class AccessoriesController : Controller
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

        // GET: Accessories
        public ActionResult Index()
        {
            if (!db.Accessories.Any())
            {
            List<Accessory> accessories = new List<Accessory>();
            accessories.Add(new Accessory
            {
                UPC = "030878735803",
                Name = "Jasco 73580 HDMI 3-Feet Cable",
                Type = "HDMI3",
                Description = "A 3 foot long HDMI Cable. Perfect for mounting your Pi near, or behind, your TV if you are using your Pi as an HTPC",
                Size = 3,
                Price = 4.79m,
                Image = "/Images/Accessories/hdmi.jpg"
            });

            accessories.Add(new Accessory
            {
                UPC = "575214360009",
                Name = "Sandisk 8GB Class 10 SD Card (Black)",
                Type = "SD8",
                Description = "The Standard size SD Card for almost any Operating System you will need to put on your Pi",
                Size = 8,
                Price = 8.65m,
                Image = "/Images/Accessories/sdCard.jpg"
            });

            db.Accessories.AddRange(accessories);
            db.SaveChanges();

            }

            if (!db.PiCases.Any())
            {
                List<PiCase> cases = new List<PiCase>();
                cases.Add(new PiCase
                {
                    UPC = "720189973338",
                    Name = "Deep Space",
                    Color = "Blackest Black",
                    Description = "Subtle, Sleek, Black case for your Pi ModelB-2 or Pi3",
                    Price = 12.99m,
                    Image = "/Images/Cases/pi3Black.jpg"
                });

                cases.Add(new PiCase
                {
                    UPC = "760853933411",
                    Name = "Clearly A Case",
                    Color = "Clear",
                    Description = "A nice clear case so that you can still see your Pi. Adafruit branding on the cover. It's like having your pie and eating it too!",
                    Price = 12.26m,
                    Image = "/Images/Cases/pi3ClearAdafruit.jpg"
                });

                cases.Add(new PiCase
                {
                    UPC = "640522710768",
                    Name = "Knights of the Pi Table",
                    Color = "Brick Red",
                    Description = "A fantastic fantasy case for your Pi ModelB-2 or Pi3",
                    Price = 13.99m,
                    Image = "/Images/Cases/pi3LegoCastle.jpg"
                });


                cases.Add(new PiCase
                {
                    UPC = "630125952818",
                    Name = "Doctorin' the Pi",
                    Color = "Blue",
                    Description = "For you Doctor Who Fans! Get your Pi into a TARDIS. We promise your Pi will stay in this time and relative dimension.",
                    Price = 14.99m,
                    Image = "/Images/Cases/pi3LegoTardis.jpg"
                });

                cases.Add(new PiCase
                {
                    UPC = "753864333806",
                    Name = "Tiny-Clear",
                    Color = "Clear",
                    Description = "A clear case for your Pi Zero",
                    Price = 8.69m,
                    Image = "/Images/Cases/piZeroClear2.jpg"
                });

                cases.Add(new PiCase
                {
                    UPC = "707565827171",
                    Name = "Pi-nky",
                    Color = "Pink and White",
                    Description = "A pink case for your Zero. Not just for girls!",
                    Price = 13.55m,
                    Image = "/Images/Cases/piZeroPinkWhite.jpg"
                });

                cases.Add(new PiCase
                {
                    UPC = "614961951136",
                    Name = "Smile at Pi-nky",
                    Color = "Pink and White",
                    Description = "Same as the Pinky Case, but make sure to smile for the built in camera!",
                    Price = 14.75m,
                    Image = "/Images/Cases/piZeroPinkWhiteCamera.jpg"
                });

                db.PiCases.AddRange(cases);
                db.SaveChanges();

            }


            Models.AccessoriesCases model = new Models.AccessoriesCases();
            model.Accessories = db.Accessories.ToArray();
            model.Cases = db.PiCases.ToArray();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection, int? value, string CurrentPiId)
        {
            Guid cartId;
            Cart c = null;
            string[] accessoriesArray = collection["Accessories"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            bool accessoriesHaveBeenAdded = false;
            for (var i = 0; i < accessoriesArray.Length; i += 1)
            {
                if (accessoriesArray[i] != "false")
                {
                    accessoriesHaveBeenAdded = true;
                }
            }

            //System.Diagnostics.Debug.WriteLine("the array: \n");
            //for (int i = 0; i < accessoriesArray.Length; i += 1)
            //{
            //    System.Diagnostics.Debug.WriteLine("\nIndex " + i + ": " );
            //    System.Diagnostics.Debug.WriteLine(accessoriesArray[i].ToString());
            //}
            //System.Diagnostics.Debug.WriteLine(value);

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
                db.Carts.Add(c);
                db.SaveChanges();
                cartId = c.CartId;
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

            if (accessoriesHaveBeenAdded)
            {
                for (int i = 0; i < accessoriesArray.Length; i += 1)
                {
                    if (accessoriesArray[i] != "false")
                    {
                    Accessory currentAccessory = new Accessory();
                    int accessoryIDFromArray = Int32.Parse(accessoriesArray[i]);
                    currentAccessory = db.Accessories.First(x => x.AccessoryId == accessoryIDFromArray);
                    currentPi.ALaModes.Add(currentAccessory);
                    }
                }
            }

            if (value != 0)
            {
                currentPi.Crust = db.PiCases.Find(value);
            }

            db.SaveChanges();

            accessoriesArray = null;


            if (currentPi.Filling == null)
            {
                return RedirectToAction("Index", "OS");
            }

            if (currentPi.Pi == null)
            {
                return RedirectToAction("Index", "Pi");
            }

            return RedirectToAction("Index", "Cart");

        }
    }
}