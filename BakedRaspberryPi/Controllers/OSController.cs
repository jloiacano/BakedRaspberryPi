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
        // GET: OSs
        public ActionResult Index()
        {
            List<OS> oss = new List<OS>();
            oss.Add(new OS
            {
                Name = "OSMC",
                Description = "Tiny and Powerful",
                Image = "/Images/OSs/osmc.png"
            });

            oss.Add(new OS
            {
                Name = "OSMC",
                Description = "Tiny and Powerful",
                Image = "/Images/OSs/debian.jpg"
            });

            oss.Add(new OS
            {
                Name = "OpenElec",
                Description = "Fast. Light. Linux!",
                Image = "/Images/OSs/openElec.png"
            });


            return View(oss);
        }
    }
}