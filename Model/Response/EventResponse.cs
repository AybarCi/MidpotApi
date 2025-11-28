using System;

namespace DatingWeb.Model.Response
{
    public class EventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int CreditsSpent { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string PlaceName { get; set; }
        public string PlaceAddress { get; set; }
        public int? Capacity { get; set; }
        public int ParticipantCount { get; set; }
        public Guid InterestId { get; set; }
        public string InterestName { get; set; }
        public long CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorPhoto { get; set; }
    }
}
