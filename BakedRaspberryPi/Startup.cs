﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.Cookies;
using Owin;
using BakedRaspberryPi.Models;

[assembly: Microsoft.Owin.OwinStartup(typeof(BakedRaspberryPi.Startup))]

namespace BakedRaspberryPi
{
    public class Startup
    {

        public void Configuration(Owin.IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/Account/LogOn")
            });

            app.CreatePerOwinContext(() =>
            {
                UserStore<BakedPiUser> store = new UserStore<BakedPiUser>();
                UserManager<BakedPiUser> manager = new UserManager<BakedPiUser>(store);
                manager.UserTokenProvider = new EmailTokenProvider<BakedPiUser>();

                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 4,
                    RequireDigit = false,
                    RequireUppercase = false,
                    RequireLowercase = false,
                    RequireNonLetterOrDigit = false
                };

                return manager;
            });
        }
    }
}