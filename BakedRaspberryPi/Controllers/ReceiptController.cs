﻿using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi.Controllers
{
    public class ReceiptController : Controller
    {
        private BakedPiModels db = new BakedPiModels();

        // GET: Receipt/Index/5
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.First(x => x.TrackingNumber == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}