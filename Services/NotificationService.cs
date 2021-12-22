using DatingWeb.Model.Request;
using DatingWeb.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHttpClientFactory _clientFactory;

        public NotificationService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task SendNotification(NotificationMessageHubRequest model)
        {
            using (var client = _clientFactory.CreateClient("notificationService"))
                await client.PostAsync("", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
        }

    }
}
