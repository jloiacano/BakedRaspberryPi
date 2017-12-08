using System;
using System.Web;
using System.Web.Mvc;
using Braintree;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RestSharp;
using RestSharp.Authenticators;

namespace BakedRaspberryPi.Controllers
{
    public class AccountController : Controller
    {

        BakedPiPaymentServices paymentServices = new BakedPiPaymentServices();        

        // GET: Account
        [Authorize]
        public ActionResult Index()
        {
            var customer = paymentServices.GetCustomer(User.Identity.Name);
            return View(customer);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(string firstName, string lastName, string id)
        {
            Braintree.Customer customer = paymentServices.UpdateCustomer(firstName, lastName, id);

            ViewBag.Message = "Updated Successfully";
            return View(customer);
        }

        [Authorize]
        public ActionResult Addresses()
        {
            var customer = paymentServices.GetCustomer(User.Identity.Name);
            return View(customer.Addresses);
        }

        [Authorize]
        public ActionResult DeleteAddress(string id)
        {
            paymentServices.DeleteAddress(User.Identity.Name, id);
            TempData["SuccessMessage"] = "Address deleted successfully";
            return RedirectToAction("Addresses");

        }

        [Authorize]
        [HttpPost]
        public ActionResult AddAddress(string firstName, string lastName, string company, string streetAddress, string extendedAddress, string locality, string region, string postalCode, string countryName)
        {

            paymentServices.AddAddress(User.Identity.Name, firstName, lastName, company, streetAddress, extendedAddress, locality, region, postalCode, countryName);

            TempData["SuccessMessage"] = "Address added successfully";
            return RedirectToAction("Addresses");
        }

        [Authorize]
        public ActionResult CreditCards()
        {
            var customer = paymentServices.GetCustomer(User.Identity.Name);
            return View(customer.CreditCards);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddCreditCard(string nameOnCard, string creditCardNumber, string expMo, string expYr, string cvv)
        {
            string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
            var gateway = new BraintreeGateway(environment, merchantId, publicKey, privateKey);

            var customer = paymentServices.GetCustomer(User.Identity.Name);
            var updater = paymentServices.UpdateCustomer(customer.FirstName, customer.LastName, customer.Id);
            string expirationDate = expMo + "/" + expYr.Substring(2,2);
            var creditCardRequest = new CreditCardRequest
            {
                CustomerId = customer.Id,
                Number = creditCardNumber,
                CardholderName = nameOnCard,
                ExpirationDate = expirationDate,
                CVV = cvv
            };

            CreditCard creditCard = gateway.CreditCard.Create(creditCardRequest).Target;

            TempData["CreditCardMessage"] = "Credit Card added successfully";

            return RedirectToAction("CreditCards");
        }

        [Authorize]
        public ActionResult DeleteCreditCard(string token)
        {
            var customer = paymentServices.GetCustomer(User.Identity.Name);
            string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
            var gateway = new BraintreeGateway(environment, merchantId, publicKey, privateKey);
            gateway.CreditCard.Delete(token);
            
            TempData["CreditCardMessage"] = "Credit Card deleted successfully";

            return RedirectToAction("CreditCards");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password)
        {
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
        
        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string piUserName, string piPassword, bool? staySignedIn)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();

            var user = userManager.FindByName(piUserName);
            if (user != null)
            {
                bool isPasswordValid = userManager.CheckPassword(user, piPassword);
                if (isPasswordValid)
                {
                    var claimsIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                    {
                        IsPersistent = staySignedIn ?? false,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    }, claimsIdentity);
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = new string[] { "Unable to sign in" };
            return View();
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                string resetToken = userManager.GeneratePasswordResetToken(user.Id);
                string resetUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword?email=" + email + "&token=" + resetToken;
                string link = string.Format("<a href=\"{0}\">Reset your password</a>", resetUrl);
                //userManager.SendEmail(user.Id, "your password reset token", message);
                EmailMessageMaker forgotPasswordEmail = new EmailMessageMaker();
                forgotPasswordEmail.Line.Add("<h2>Thank you for contacting us!</h2>");
                forgotPasswordEmail.Line.Add("<p>To reset your password, please click on the link below.</p>");
                forgotPasswordEmail.Line.Add(link);
                forgotPasswordEmail.Line.Add("<p>Have a great day!</p>");
                PiMailer passwordReset = new PiMailer(user.Email, "Instructions to reset your BakedRaspberryPi password", forgotPasswordEmail);
                passwordReset.SendMail();
            }

            return RedirectToAction("ForgotPasswordSent");
        }

        public ActionResult ForgotPasswordSent()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string email, string token, string newPassword)
        {

            var userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            var user = userManager.FindByEmail(email);
            if (user != null)
            {
                IdentityResult result = userManager.ResetPassword(user.Id, token, newPassword);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Your password has been updated successfully";
                    return RedirectToAction("SignIn", "Account");
                }

            }
            return RedirectToAction("Index", "Home");
        }
    }
}