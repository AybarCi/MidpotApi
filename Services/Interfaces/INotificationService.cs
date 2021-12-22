using DatingWeb.Model.Request;
using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotification(NotificationMessageHubRequest model);
    }
}
