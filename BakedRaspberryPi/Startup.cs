using System;
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
                LoginPath = new Microsoft.Owin.PathString("/Account/SignIn")
            });

            app.CreatePerOwinContext(() =>
            {
                UserStore<IdentityUser> store = new UserStore<IdentityUser>(new BakedPiModels());
                UserManager<IdentityUser> manager = new UserManager<IdentityUser>(store)
                {
                    UserTokenProvider = new EmailTokenProvider<IdentityUser>(),

                    PasswordValidator = new PasswordValidator
                    {
                        RequiredLength = 4,
                        RequireDigit = false,
                        RequireUppercase = false,
                        RequireLowercase = false,
                        RequireNonLetterOrDigit = false
                    },

                    EmailService = new BakedRaspberryPiEmailService()
                };

                return manager;
            });
        }
    }
}