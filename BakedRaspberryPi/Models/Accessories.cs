using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi.Models
{
    public class Accessories
    {
        public Accessories() { }

        public string UPC { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Size { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }

        public static Accessories[] SampleData
        {
            get
            {
                List<Accessories> accsessory = new List<Accessories>();
                accsessory.Add(new Accessories
                {
                    UPC = "030878735803",
                    Name = "Jasco 73580 HDMI 3-Feet Cable",
                    Type = "HDMI3",
                    Description = "A 3 foot long HDMI Cable. Perfect for mounting your Pi near, or behind, your TV if you are using your Pi as an HTPC",
                    Size = 3,
                    Price = 4.79m,
                    Image = "/Images/Accessories/hdmi.jpg"
                });

                accsessory.Add(new Accessories
                {
                    UPC = "575214360009",
                    Name = "Sandisk 8GB Class 10 SD Card (Black)",
                    Type = "SD8",
                    Description = "The Standard size SD Card for almost any Operating System you will need to put on your Pi",
                    Size = 8,
                    Price = 8.65m,
                    Image = "/Images/Accessories/sdCard.jpg"
                });
                return accsessory.ToArray();
            }
        }
    }
}