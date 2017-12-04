using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System;
using RestSharp;
using RestSharp.Authenticators;

namespace BakedRaspberryPi
{
    internal class BakedRaspberryPiMailGunService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string MailGunApiKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.ApiKey"];
            string MailGunPubKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PubKey"];
            string MailGunSandBox = System.Configuration.ConfigurationManager.AppSettings["Mailgun.SandBox"];
            string MailGunPostMan = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PostMan"];
            string MailGunTestRecipient = System.Configuration.ConfigurationManager.AppSettings["Mailgun.TestRecipient"];

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", MailGunApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", MailGunSandBox, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@"+MailGunSandBox+">");
            request.AddParameter("to", MailGunTestRecipient + " <" + MailGunTestRecipient + ">");
            request.AddParameter("subject", "Hello " + MailGunTestRecipient);
            request.AddParameter("text", "This has been sent to you from MailGun Mailing service");
            request.Method = Method.POST;
            return (Task)client.Execute(request);
            throw new NotImplementedException();
        }
    }
}