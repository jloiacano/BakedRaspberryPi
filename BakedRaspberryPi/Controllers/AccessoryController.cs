using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class AccessoryController : Controller
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
            if (!db.Carts.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            if (!db.Accessories.Any())
            {
            List<Accessory> accessories = new List<Accessory>();
                accessories.Add(new Accessory
                {
                    UPC = "XXXX",
                    Name = "NONE",
                    Type = "",
                    Description = "",
                    Size = 0,
                    Price = 0m,
                    Image = ""
                });

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
                    UPC = "XXXX",
                    Name = "NONE",
                    Color = "",
                    Description = "",
                    Price = 0m,
                    Image = ""
                });

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
            bool accessoriesAreBeingEdited = false;
            List<int> previousAccessories = new List<int>();
            bool casesAreBeingEdited = false;

            for (var i = 0; i < accessoriesArray.Length; i += 1)
            {
                if (accessoriesArray[i] != "false")
                {
                    accessoriesHaveBeenAdded = true;
                }
            }
            
            if (Request.Cookies.AllKeys.Contains("cartId"))
            {
                cartId = Guid.Parse(Request.Cookies["cartId"].Value);
                c = db.Carts.Find(cartId);
            }
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

            foreach (var checkAccessory in c.WholePis)
            {
                if (checkAccessory.IsEdit == true)
                {
                    accessoriesAreBeingEdited = true;
                }
            }

            foreach (var currentAccessory in db.Accessories)
            {
                if (currentAccessory.IsEdit == true)
                {
                    previousAccessories.Add(currentAccessory.AccessoryId);
                }
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
                        currentPi.Price += currentAccessory.Price;
                    }
                }
            }
            else
            {
                Accessory currentAccessory = new Accessory();
                currentAccessory = db.Accessories.Find(1);
                currentPi.ALaModes.Add(currentAccessory);
            }

            if (value == null)
            {
                currentPi.Crust = db.PiCases.Find(1);
            }
            else
            {
                currentPi.Crust = db.PiCases.Find(value);
                currentPi.Price += currentPi.Crust.Price;
            }

            db.SaveChanges();

            accessoriesArray = null;

            return RedirectToAction("Index", "Cart");

        }

        public ActionResult EditAccessories(WholePi WholePiPi, int wholePiToBeEditedId, string previousAccessoriesIds)
        {
            var prevIds = previousAccessoriesIds.Split(',').Select(x => int.Parse(x));

            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            foreach (var accessory in toEdit.ALaModes)
            {
                accessory.IsEdit = true;
            }
            foreach (var currentAccessory in db.Accessories)
            {
                foreach (var id in prevIds)
                {
                    if (id == currentAccessory.AccessoryId)
                    {
                        currentAccessory.IsEdit = true;
                    }

                }
            }
            db.SaveChanges();

            return RedirectToAction("Index", "Accessory");
        }


        public ActionResult EditCases(WholePi WholePiPi, int wholePiToBeEditedId, int previousPiCaseId)
        {
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            toEdit.Crust.IsEdit = true;
            toEdit.Crust.EditPreviousId = previousPiCaseId;
            db.SaveChanges();
            return RedirectToAction("Index", "Accessory");
        }
    }
}