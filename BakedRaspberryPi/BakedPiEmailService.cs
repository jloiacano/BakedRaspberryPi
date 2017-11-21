using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BakedRaspberryPi
{
    internal class BakedPiEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["SendGrid.key"];
            SendGrid.SendGridClient client = new SendGrid.SendGridClient(apiKey);

            SendGrid.Helpers.Mail.SendGridMessage mail = new SendGrid.Helpers.Mail.SendGridMessage();
            mail.SetFrom(new SendGrid.Helpers.Mail.EmailAddress { Name = "J Loiacano", Email = "j.loiacano@bakedPi.com" });
            mail.AddTo(message.Destination);
            mail.Subject = message.Subject;
            mail.AddContent("text/plain", message.Body);
            mail.SetTemplateId("ba7cd215-f0d3-475c-bd63-a288b63818e1");

            throw new System.NotImplementedException();
        }
    }
}