using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Helper;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Chat.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Chat
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
        }

        public async Task<long> SendMessage(long userId, long matchId, string message, string ipAddress, DateTime dateTime)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            Message mess = new Message { UserId = userId, CreateDate = dateTime, MatchId = matchId, Chat = message, IpAddress = ipAddress };
            await _context.Message.AddAsync(mess);
            await _context.SaveChangesAsync();
            return mess.MessageId;
        }
        public async Task<bool> ReadMessages(long matchId, long matchedUserId)
        {
            var messages = _context.Message.Where(d => d.MatchId == matchId && d.UserId == matchedUserId && d.IsRead != true).ToList();
            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    message.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
            
            return true;
        }

        public async Task<bool> ReadMessage(long messageId)
        {
            var message = await _context.Message.Where(d => d.MessageId == messageId).FirstOrDefaultAsync();
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<List<MessageResponse>> LoadConversation(long userId, long matchId, long lastUpdateId)
        {
            var conversation = await _context.Match.Where(x => x.MatchId == matchId && x.IsActive == true).FirstOrDefaultAsync();
            if (conversation != null)
            {
                if (conversation.FemaleUser == userId || conversation.MaleUser == userId)
                {
                    return await _context.Message.Where(x => x.MatchId == matchId &&
                    (x.UserId == conversation.FemaleUser || x.UserId == conversation.MaleUser)
                    && x.MessageId > lastUpdateId).Select(x => new MessageResponse
                    {
                        Chat = x.Chat,
                        CreateDate = x.CreateDate,
                        UserId = x.UserId,
                        MessageId = x.MessageId
                    }).OrderBy(x => x.MessageId).Take(100).ToListAsync();
                }
                return null;
            }
            else
            {
                //Kullanıcı engellenmiş!
                throw new ApiException("333");
            }
        }

        public async Task<List<NewMessagesResponse>> GetNewMessages(long userId)
        {
            var matches = await _context.Match.Where(x => x.FemaleUser == userId || x.MaleUser == userId && x.IsActive == true).Select(x=> x.MatchId).ToListAsync();
            if (matches.Count > 0)
            {
                return await _context.Message.Where(x => matches.Contains(x.MatchId) && x.IsRead == false && x.UserId != userId).GroupBy(x => x.UserId).Select(
                        x => new NewMessagesResponse
                        {
                            UserId = x.Key,
                            MessageCount = x.Count()
                        }
                    ).ToListAsync();
            }
            else { return null; }
        }
    }
}
