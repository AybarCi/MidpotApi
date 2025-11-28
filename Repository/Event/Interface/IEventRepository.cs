using DatingWeb.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Event.Interface
{
    public interface IEventRepository
    {
        Task<DatingWeb.Data.DbModel.Event> CreateEventAsync(DatingWeb.Data.DbModel.Event @event);
        Task<DatingWeb.Data.DbModel.Event> GetEventByIdAsync(Guid id);
        Task<IEnumerable<DatingWeb.Data.DbModel.Event>> GetEventsByInterestAsync(Guid interestId);
        Task<IEnumerable<DatingWeb.Data.DbModel.Event>> GetLatestEventsAsync(int count);
        Task UpdateEventAsync(DatingWeb.Data.DbModel.Event @event);
        Task AddParticipantAsync(EventParticipant participant);
        Task<EventParticipant> GetParticipantAsync(Guid eventId, long userId);
        Task<int> GetParticipantCountAsync(Guid eventId);
    }
}
