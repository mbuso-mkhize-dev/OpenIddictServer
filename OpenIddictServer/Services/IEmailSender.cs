using System.Threading.Tasks;

namespace OpenIddictServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
