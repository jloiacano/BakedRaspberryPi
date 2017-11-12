using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi.Models
{
    public class WholePi
    {
        public bool isInstantiated = false;
        public int ID { get; set; }
        public Pi Pi { get; set; }
        public OS Filling { get; set; }
        public PiCase Crust { get; set; }
        public Accessories[] ALaModes { get; set; }
        public decimal Price { get; set; }

        //
        // TODO something with this....
        public static WholePi PrepAPi()
        {
            WholePi aPi = new WholePi();
            aPi.Pi = new Pi();
            
            return aPi;
        }
    }
}