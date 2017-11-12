using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BakedRaspberryPi.Models;

namespace BakedRaspberryPi.Controllers
{
    public class WholePiController : Controller
    {
        // GET: BakedPi
        public ActionResult Index(int id)
        {
            var aWholePi = new WholePi();

            return View();
        }
    }
}