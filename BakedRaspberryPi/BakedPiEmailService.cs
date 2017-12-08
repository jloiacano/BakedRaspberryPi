using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BakedRaspberryPi
{
    public class BakedPiEmailService : IIdentityMessageService
    {

        private string MailGunApiKey = System.Configuration.ConfigurationManager.AppSettings["Mailgun.ApiKey"];
        private string MailGunSandBox = System.Configuration.ConfigurationManager.AppSettings["Mailgun.SandBox"];
        private string MailGunPostMan = System.Configuration.ConfigurationManager.AppSettings["Mailgun.PostMan"];

        public Task SendAsync(IdentityMessage message)
        {
            string apiKey = MailGunApiKey;
            string sandBox = MailGunSandBox;
            byte[] apiKeyAuth = Encoding.ASCII.GetBytes($"api:{apiKey}");
            var httpClient = new HttpClient { BaseAddress = new Uri("https://api.mailgun.net/v3/") };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(apiKeyAuth));

            var form = new Dictionary<string, string>
            {
                ["from"] = MailGunPostMan,
                ["to"] = message.Destination,
                ["subject"] = message.Subject,
                ["text"] = message.Body
            };

            HttpResponseMessage response =
                httpClient.PostAsync(sandBox + "/messages", new FormUrlEncodedContent(form)).Result;
            return Task.FromResult((int)response.StatusCode);
        }
    }
}