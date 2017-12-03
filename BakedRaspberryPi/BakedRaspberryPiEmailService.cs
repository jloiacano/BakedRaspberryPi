using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System;

namespace BakedRaspberryPi
{
    internal class BakedRaspberryPiEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["SendGrid.key"];
            SendGrid.SendGridClient client = new SendGrid.SendGridClient(apiKey);

            SendGrid.Helpers.Mail.SendGridMessage mail = new SendGrid.Helpers.Mail.SendGridMessage();
            mail.SetFrom(new SendGrid.Helpers.Mail.EmailAddress { Name = "J Loiacano", Email = "j.loiacano@bakedPi.com" });
            mail.AddTo(message.Destination);
            mail.SetSubject(message.Subject);
            mail.AddContent("text/plain", message.Body);
            mail.AddContent("text/html", message.Body);

            mail.SetTemplateId("ba7cd215-f0d3-475c-bd63-a288b63818e1");

            mail.AddSubstitution("<%copyright%>", string.Format("©{0} Loiacano Designs", DateTime.Now.Year.ToString()));
            
            return client.SendEmailAsync(mail);
            throw new System.NotImplementedException();
        }
    }
}