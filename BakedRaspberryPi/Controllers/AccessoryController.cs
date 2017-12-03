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

                accessories.Add(new Accessory
                {
                    UPC = "608763516389",
                    Name = "Official Raspberry Pi Power Adapter",
                    Type = "power",
                    Description = "5v 2a (2000ma) Micro Usb Adapter Charger Ac Power Supply For Raspberry Pi Black",
                    Size = 5,
                    Price = 6.18m,
                    Image = "/Images/Accessories/powerAdapter.jpg"
                });

                accessories.Add(new Accessory
                {
                    UPC = "682710990729",
                    Name = "Raspberry PI 5MP Camera Board Module",
                    Type = "camera",
                    Description = "Raspberry Pi Camera Module w/ Adjustable Focus and Night Vision",
                    Size = 5,
                    Price = 22.99m,
                    Image = "/Images/Accessories/camera.jpg"
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
            
            Models.AccessoriesCases model = new Models.AccessoriesCases
            {
                Accessories = db.Accessories.ToArray(),
                Cases = db.PiCases.ToArray()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection, int? value, string CurrentPiId)
        {
            Guid cartId = Guid.Parse(Request.Cookies["cartId"].Value);
            Cart c = db.Carts.Find(cartId);
            int WholePiToBeEdited = 0;
            decimal accessoriesPriceToBeDeducted = 0m;
            decimal casesPriceToBeDeducted = 0m;
            bool accessoriesHaveBeenAdded = false;
            bool accessoriesAreBeingEdited = false;
            List<int> previousAccessories = new List<int>();
            bool casesAreBeingEdited = false;
            int[] accessoriesArray;

            #region set the vars
            // if there are accessories checked on the page, set them to accessoriesArray.
            // Otherwise set accessories array to an empty array.
            if (collection["Accessories"] != null)
            {

                accessoriesArray = collection["Accessories"].Split(',').Select(Int32.Parse).ToArray();
                accessoriesHaveBeenAdded = true;
            }
            else
            {
                List<int> temporaryList = new List<int>();
                accessoriesArray = temporaryList.ToArray();
            }
            
            // set a variable of which wholePi is to be edited
            foreach (var currentWholePi in c.WholePis)
            {
                if (currentWholePi.IsEdit == true)
                {
                    WholePiToBeEdited = currentWholePi.WholePiId;
                    currentWholePi.IsEdit = false;
                }
            }            
            #endregion

            #region get the WholePi
            // Pull up the correct currentPi
            WholePi currentPi;
            if (WholePiToBeEdited != 0)
            {
                currentPi = c.WholePis.FirstOrDefault(x => x.WholePiId == WholePiToBeEdited);

                // check if it is the accessories being edited (as opposed to the piCases)
                // and add to the previousAccessoires List which accessories were already in this wholePi
                // and total up the price of those previous accessories and reset the IsEdit on it.
                foreach (var currentAccessory in currentPi.ALaModes)
                {
                    if (currentAccessory.IsEdit == true)
                    {
                        if (currentAccessory.AccessoryId != 0)
                        {
                            accessoriesAreBeingEdited = true;
                            previousAccessories.Add(currentAccessory.AccessoryId);
                            accessoriesPriceToBeDeducted += currentAccessory.Price;
                        }
                        currentAccessory.IsEdit = false;
                    }
                }

                // check if it is the piCase that is being edited, and if so, set the price to be deducted
                // and reset the IsEdit on it.
                if (currentPi.Crust.IsEdit == true)
                {
                    casesAreBeingEdited = true;
                    casesPriceToBeDeducted += currentPi.Crust.Price;
                    currentPi.Crust.IsEdit = false;
                    currentPi.Crust.EditPreviousId = 0;
                }
            }
            else if (c.CurrentPiId == 0)
            {
                currentPi = c.WholePis.FirstOrDefault();
            }
            else
            {
                currentPi = c.WholePis.First(x => x.WholePiId == c.CurrentPiId);
            }

            if (accessoriesAreBeingEdited)
            {
                currentPi.ALaModes.Clear();
            }
            #endregion
            
            if (accessoriesHaveBeenAdded && !casesAreBeingEdited)
            {
                for (int i = 0; i < accessoriesArray.Length; i += 1)
                {
                    currentPi.ALaModes.Add(db.Accessories.Find(accessoriesArray[i]));
                }
                currentPi.Price += currentPi.ALaModes.Sum(x => x.Price);
                currentPi.Price -= accessoriesPriceToBeDeducted;
            }
            else if (accessoriesHaveBeenAdded)
            {
                Accessory currentAccessory = new Accessory();
                currentAccessory = db.Accessories.Find(1);
                currentPi.ALaModes.Add(currentAccessory);
            }
            
            if (!accessoriesAreBeingEdited)
            {
                if (value == null)
                {
                    currentPi.Crust = db.PiCases.Find(1);
                }
                else
                {
                    currentPi.Crust = db.PiCases.Find(value);
                    currentPi.Price += currentPi.Crust.Price;
                    currentPi.Price -= casesPriceToBeDeducted;
                }
            }

            db.SaveChanges();
            
            return RedirectToAction("Index", "Cart");

        }

        public ActionResult EditAccessories(WholePi WholePiPi, int wholePiToBeEditedId, string previousString)
        {
            int[] prevIds;
            if (previousString == null || previousString == "")
            {
                prevIds = new int[] { 0 };
            }
            else
            {
                prevIds = previousString.Split(',').Select(x => int.Parse(x)).ToArray();
            }

            // Get the right WholePi and set it's IsEdit to true.
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            
            if (toEdit.ALaModes.Count() == 0)
            {
                toEdit.ALaModes.Add(db.Accessories.Find(1));
                toEdit.ALaModes.First().IsEdit = true;
            }

            // Go through the accessories that are in the right WholePi and set their IsEdit to true.
            foreach (var accessory in toEdit.ALaModes)
            {
                if (accessory.AccessoryId != 0)
                {
                    accessory.IsEdit = true;
                }
            }
            
            // Go through all the accessories in the database and set the ones that are in the right WholePi and set
            // their IsEdit to true, so that when the index views the accessories, it can place checkboxes on the right ones.
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


        public ActionResult Edit(WholePi WholePiPi, int wholePiToBeEditedId, int? previous)
        {
            WholePi toEdit = db.WholePis.FirstOrDefault(x => x.WholePiId == wholePiToBeEditedId);
            toEdit.IsEdit = true;
            toEdit.Crust.IsEdit = true;
            if (previous != null)
            {
                toEdit.Crust.EditPreviousId = (int)previous;
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Accessory");
        }
    }
}