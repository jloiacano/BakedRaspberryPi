using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BakedRaspberryPi.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password)
        {
            //Make sure the following statements are in the using block:
            //using Microsoft.AspNet.Identity;
            //using Microsoft.AspNet.Identity.EntityFramework;
            //using Microsoft.AspNet.Identity.Owin;
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            IdentityUser user = new IdentityUser { Email = username, UserName = username };

            IdentityResult result = userManager.Create(user, password);
            if (result.Succeeded)
            {
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                }, userIdentity);

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = result.Errors;
            return View();
        }
    }
}