using DatingWeb.Model.Request;
using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface ISmsService
    {
        Task<bool> SendSms(SendSmsRequest request);
    }
}
