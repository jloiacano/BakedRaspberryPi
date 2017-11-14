using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi.Models
{
    public class PiCase
    {
        public PiCase() { }

        public string UPC { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }

        public static PiCase[] SampleData
        {
            get
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
                return cases.ToArray();
            }
        }
    }


}