using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Repository.Event.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Event
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DatingWeb.Data.DbModel.Event> CreateEventAsync(DatingWeb.Data.DbModel.Event @event)
        {
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
            return @event;
        }

        public async Task<DatingWeb.Data.DbModel.Event> GetEventByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.Creator)
                .Include(e => e.Interest)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<DatingWeb.Data.DbModel.Event>> GetEventsByInterestAsync(Guid interestId)
        {
            return await _context.Events
                .Where(e => e.InterestId == interestId && e.Status == EventStatus.Published)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<DatingWeb.Data.DbModel.Event>> GetLatestEventsAsync(int count)
        {
             return await _context.Events
                .Where(e => e.Status == EventStatus.Published)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task UpdateEventAsync(DatingWeb.Data.DbModel.Event @event)
        {
            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
        }

        public async Task AddParticipantAsync(EventParticipant participant)
        {
            await _context.EventParticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<EventParticipant> GetParticipantAsync(Guid eventId, long userId)
        {
            return await _context.EventParticipants
                .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        }
        
        public async Task<int> GetParticipantCountAsync(Guid eventId)
        {
            return await _context.EventParticipants
                .CountAsync(ep => ep.EventId == eventId && (ep.Status == "joined" || ep.Status == "invited"));
        }
    }
}
