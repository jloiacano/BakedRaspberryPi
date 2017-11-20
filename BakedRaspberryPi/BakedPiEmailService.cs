using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace BakedRaspberryPi
{
    internal class BakedPiEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}