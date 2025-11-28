using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventResponse> CreateEventAsync(long userId, CreateEventRequest request);
        Task<IEnumerable<EventResponse>> GetLatestEventsAsync(Guid? interestId);
        Task JoinEventAsync(Guid eventId, long userId);
        Task InviteUserAsync(Guid eventId, long inviterId, long inviteeId);
    }
}
