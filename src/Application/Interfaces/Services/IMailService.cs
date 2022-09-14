using EDO_FOMS.Application.Interfaces.Common;
using EDO_FOMS.Application.Requests.Mail;
using System.Threading.Tasks;

namespace EDO_FOMS.Application.Interfaces.Services
{
    public interface IMailService : IService
    {
        Task SendAsync(MailRequest request);
    }
}