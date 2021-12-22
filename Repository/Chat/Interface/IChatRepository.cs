using DatingWeb.Model;
using DatingWeb.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Chat.Interface
{
    public interface IChatRepository
    {
        Task<long> SendMessage(long userId, long matchId, string message, string ipAddress, DateTime dateTime);
        Task<List<MessageResponse>> LoadConversation(long userId, long matchId, long lastUpdateId);
        Task<List<NewMessagesResponse>> GetNewMessages(long userId, DateTime LastSawMessagesDate);
    }
}
