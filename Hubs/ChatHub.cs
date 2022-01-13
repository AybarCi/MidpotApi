using DatingWeb.CacheService.Interface;
using DatingWeb.Repository.Chat.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DatingWeb.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICache _cache;

        public ChatHub(IChatRepository chatRepository, IHttpContextAccessor httpContextAccessor, ICache cache)
        {
            _chatRepository = chatRepository;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task GetConnectionId(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task SendToChannel(string matchId, string userId, string message)
        {
            if (_cache.Get(matchId) == null)
            {
                DateTime dateTimeNow = DateTime.UtcNow;
                string ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                long messageId = await _chatRepository.SendMessage(long.Parse(userId), long.Parse(matchId), message, ipAddress, dateTimeNow);

                //Signalr soket
                await Clients.Group(matchId).SendAsync("SendToChannel", userId, message, dateTimeNow, messageId);
            }
            else
                await Clients.Group(matchId).SendAsync("SendToChannel", userId, "", DateTime.UtcNow, "-1");
        }
    }
}
