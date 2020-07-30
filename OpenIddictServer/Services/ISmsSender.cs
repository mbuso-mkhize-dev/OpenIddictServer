using System.Threading.Tasks;

namespace OpenIddictServer.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
