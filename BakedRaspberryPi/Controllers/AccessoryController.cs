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
            
            if (WholePiToBeEdited != 0)
            {
                // check if it is the accessories being edited (as opposed to the piCases)
                // and add to the previousAccessoires List which accessories were already in this wholePi
                // and total up the price of those previous accessories and reset the IsEdit on it.
                foreach (var currentAccessory in db.Accessories)
                {
                    if (currentAccessory.IsEdit == true)
                    {
                        accessoriesAreBeingEdited = true;
                        previousAccessories.Add(currentAccessory.AccessoryId);
                        accessoriesPriceToBeDeducted += currentAccessory.Price;
                        currentAccessory.IsEdit = false;
                    }
                }

                // check if it is the piCases that are being edited, and if so, set the price to be deducted
                // and reset the IsEdit on it.
                foreach (var currentWholePi in db.WholePis)
                {
                    if (currentWholePi.Crust.IsEdit == true)
                    {
                        casesAreBeingEdited = true;
                        casesPriceToBeDeducted += currentWholePi.Crust.Price;
                        currentWholePi.Crust.IsEdit = false;
                    }
                }
            }
            #endregion

            #region get the WholePi
            // Pull up the correct currentPi
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
            else
            {
                Accessory currentAccessory = new Accessory();
                currentAccessory = db.Accessories.Find(1);
                currentPi.ALaModes.Add(currentAccessory);
            }
            

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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