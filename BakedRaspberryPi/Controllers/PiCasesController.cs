using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BakedRaspberryPi.Models;

namespace BakedRaspberryPi.Controllers
{
    public class PiCasesController : Controller
    {
        // GET: PiCases
        public ActionResult Index()
        {
            return View(PiCase.SampleData);
        }
    }
}