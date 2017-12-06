using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi
{
    public class PiMailer
    {
        private string MailGunApiKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.ApiKey"];
        private string MailGunSandBox = System.Configuration.ConfigurationManager.AppSettings["Mailgun.SandBox"];
        private string MailGunPostMan = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PostMan"];

        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public PiMailer() { }

        public PiMailer(string recipient, string subject, string body)
        {
            Recipient = recipient;
            Subject = subject;
            Body = body;
        }
        
        /// <summary>
        /// This function sends mail. The imputs are pretty self explanatory.
        /// </summary>
        public RestResponse SendMail()
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api", MailGunApiKey)
            };
            RestRequest request = new RestRequest();
            request.AddParameter("domain", MailGunSandBox, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@" + MailGunSandBox + ">");
            request.AddParameter("to", "RecipientName <" + Recipient + ">");
            request.AddParameter("subject", Subject);
            request.AddParameter("html", Body);
            request.Method = Method.POST;
            RestResponse restResponse = (RestResponse)client.Execute(request);
            return restResponse;
            throw new NotImplementedException();
        }
    }
}