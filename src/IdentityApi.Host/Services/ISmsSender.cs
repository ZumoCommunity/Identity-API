using System.Threading.Tasks;

namespace IdentityApi.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
