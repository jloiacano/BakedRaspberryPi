using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class AccessoriesController : Controller
    {
        // GET: Accessories
        public ActionResult Index()
        {
            Models.AccessoriesCases model = new Models.AccessoriesCases();
            model.Accessories = Models.Accessories.SampleData;
            model.Cases = Models.PiCase.SampleData;
            return View(model);
        }
    }
}