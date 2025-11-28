using DatingWeb.CacheService.Interface;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Event.Interface;
using DatingWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICreditService _creditService;
        private readonly IRedisCache _redisCache;
        private readonly IPlacesService _placesService;

        public EventService(
            IEventRepository eventRepository, 
            ICreditService creditService,
            IRedisCache redisCache,
            IPlacesService placesService)
        {
            _eventRepository = eventRepository;
            _creditService = creditService;
            _redisCache = redisCache;
            _placesService = placesService;
        }

        public async Task<EventResponse> CreateEventAsync(long userId, CreateEventRequest request)
        {
            // 1. Validate place with Google Places API
            var placeDetails = await _placesService.GetPlaceDetailsAsync(request.PlaceId);
            if (placeDetails != null)
            {
                // Update with validated data from Google
                request.PlaceName = placeDetails.Name;
                request.PlaceAddress = placeDetails.Address;
                request.Lat = placeDetails.Lat;
                request.Lng = placeDetails.Lng;
            }

            // 2. Check credits (1 credit to create event)
            int cost = 1;
            bool deducted = await _creditService.DeductCreditsAsync(userId, cost, "create_event", "{}");
            if (!deducted)
            {
                throw new InvalidOperationException("Insufficient credits");
            }

            // 2. Create Event
            var newEvent = new Event
            {
                CreatorId = userId,
                Title = request.Title,
                Description = request.Description,
                InterestId = request.InterestId,
                PlaceId = request.PlaceId,
                PlaceName = request.PlaceName,
                PlaceAddress = request.PlaceAddress,
                Lat = request.Lat,
                Lng = request.Lng,
                StartsAt = request.StartsAt,
                EndsAt = request.EndsAt,
                Capacity = request.Capacity,
                CreditsSpent = cost,
                Status = EventStatus.Published
            };

            var createdEvent = await _eventRepository.CreateEventAsync(newEvent);

            // 3. Add Creator as Participant (Joined)
            var participant = new EventParticipant
            {
                EventId = createdEvent.Id,
                UserId = userId,
                Status = "joined",
                JoinedAt = DateTime.UtcNow
            };
            await _eventRepository.AddParticipantAsync(participant);

            // 4. Invalidate caches
            await InvalidateEventCachesAsync(request.InterestId);

            return MapToResponse(createdEvent);
        }

        public async Task<IEnumerable<EventResponse>> GetLatestEventsAsync(Guid? interestId)
        {
            // Use cache with 30 second TTL
            var cacheKey = interestId.HasValue 
                ? $"events:interest:{interestId.Value}:latest" 
                : "events:global:latest";

            var cachedEvents = await _redisCache.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    IEnumerable<Event> events;
                    if (interestId.HasValue)
                    {
                        events = await _eventRepository.GetEventsByInterestAsync(interestId.Value);
                    }
                    else
                    {
                        events = await _eventRepository.GetLatestEventsAsync(20);
                    }
                    return events.Select(MapToResponse).ToList();
                },
                TimeSpan.FromSeconds(30)
            );

            return cachedEvents;
        }

        public async Task JoinEventAsync(Guid eventId, long userId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            if (@event == null) throw new KeyNotFoundException("Event not found");

            if (@event.Capacity.HasValue)
            {
                var count = await _eventRepository.GetParticipantCountAsync(eventId);
                if (count >= @event.Capacity.Value)
                {
                    throw new InvalidOperationException("Event is full");
                }
            }

            var existing = await _eventRepository.GetParticipantAsync(eventId, userId);
            if (existing != null)
            {
                if (existing.Status == "joined") return; // Already joined
                existing.Status = "joined";
            }
            else
            {
                await _eventRepository.AddParticipantAsync(new EventParticipant
                {
                    EventId = eventId,
                    UserId = userId,
                    Status = "joined"
                });
            }

            // Invalidate caches
            await InvalidateEventCachesAsync(@event.InterestId);
        }

        public async Task InviteUserAsync(Guid eventId, long inviterId, long inviteeId)
        {
            var @event = await _eventRepository.GetEventByIdAsync(eventId);
            if (@event == null) throw new KeyNotFoundException("Event not found");

            var existing = await _eventRepository.GetParticipantAsync(eventId, inviteeId);
            if (existing != null) return; // Already participant

            await _eventRepository.AddParticipantAsync(new EventParticipant
            {
                EventId = eventId,
                UserId = inviteeId,
                Status = "invited"
            });

            // Send push notification (simplified - would need to fetch user device token in production)
            // TODO: Fetch invitee's device token and send actual FCM notification
            // For now, logging the intent
        }

        private async Task InvalidateEventCachesAsync(Guid interestId)
        {
            // Invalidate both interest-specific and global caches
            await _redisCache.RemoveAsync($"events:interest:{interestId}:latest");
            await _redisCache.RemoveAsync("events:global:latest");
        }

        private EventResponse MapToResponse(Event e)
        {
            return new EventResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Status = e.Status.ToString(),
                CreditsSpent = e.CreditsSpent,
                StartsAt = e.StartsAt,
                EndsAt = e.EndsAt,
                PlaceName = e.PlaceName,
                PlaceAddress = e.PlaceAddress,
                Capacity = e.Capacity,
                InterestId = e.InterestId,
                InterestName = e.Interest?.Name,
                CreatorId = e.CreatorId ?? 0,
                CreatorName = e.Creator?.PersonName,
                CreatorPhoto = e.Creator?.ProfilePhoto
            };
        }
    }
}
