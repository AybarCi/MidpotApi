using DatingWeb.CacheService.Interface;
using DatingWeb.Model.Request;
using DatingWeb.Repository.User.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly ICache _cache;
        public NotificationController(INotificationService notificationService, ICache cache, IUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _cache = cache;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationMessageHubRequest model)
        {
            if (_cache.Get(model.data.matchId.ToString()) == null)
            {
                var cacheModel = _cache.Get<string>(model.to);
                string deviceToken = string.Empty;

                if (cacheModel == null)
                {
                    deviceToken = await _userRepository.GetDeviceToken(long.Parse(model.to));
                    _cache.Set(model.to, deviceToken);
                }
                else
                    deviceToken = cacheModel.ToString();

                if (!string.IsNullOrEmpty(deviceToken))
                {
                    await _notificationService.SendNotification(new NotificationMessageHubRequest
                    {
                        to = deviceToken,
                        data = new MessageDataHub
                        {
                            matchId = model.data.matchId
                        },
                        notification = new Notification
                        {
                            title = model.notification.title,
                            body = model.notification.body
                        }
                    });
                }
            }
            return Ok("ok");
        }

        //[HttpPost("send")]
        //public async Task<IActionResult> Send([FromBody] NotificationMessageRequest model)
        //{
        //    var cacheModel = _cache.Get<string>(model.to);
        //    string deviceToken = string.Empty;
        //    if (cacheModel == null)
        //    {
        //        deviceToken = await _userRepository.GetDeviceToken(long.Parse(model.to));
        //        _cache.Set(model.to, deviceToken);
        //    }
        //    else
        //        deviceToken = cacheModel.ToString();

        //    await _notificationService.SendNotification(new NotificationMessageRequest
        //    {
        //        to = deviceToken,
        //        data = new MessageData
        //        {
        //            //conversationId = model.data.conversationId.ToString(),
        //            chat = model.data.chat,
        //            userId = model.data.userId,
        //            createDate = model.data.createDate
        //        },
        //        notification = new Notification
        //        { body = model.notification.body, title = model.notification.title }
        //    });
        //    return Ok("ok");
        //}
    }
}
