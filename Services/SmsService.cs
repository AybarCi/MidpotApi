using DatingWeb.Model.Request;
using DatingWeb.Services.Interfaces;
using DatingWeb.Settings;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class SmsService : ISmsService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SmsServiceSettings _smsServiceSettings;

        public SmsService(IHttpClientFactory clientFactory, SmsServiceSettings smsServiceSettings)
        {
            _clientFactory = clientFactory;
            _smsServiceSettings = smsServiceSettings;
        }
        public async Task<bool> SendSms(SendSmsRequest request)
        {
            var body = new SmsMessageRequest
            {
                api_id = _smsServiceSettings.UserId,
                api_key = _smsServiceSettings.Password,
                sender = _smsServiceSettings.Sender,
                message_type = "turkce",
                message = string.Format("Midpot, işlemi tamamlayabilmeniz için onay kodunuz : {0}", request.ConfirmCode),
                phones = new string[1] { request.PhoneNumber }
            };

            using (var client = _clientFactory.CreateClient("smsService"))
            {
                var response = await client.PostAsync("", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    //var responseString = await response.Content.ReadAsStringAsync();
                    return true;
                }
            }
            return false;
        }
    }
}
