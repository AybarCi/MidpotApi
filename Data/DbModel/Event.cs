using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public enum EventStatus
    {
        Draft,
        Published,
        Cancelled,
        Finished
    }

    public class Event
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public long? CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid InterestId { get; set; }
        [ForeignKey("InterestId")]
        public virtual Interest Interest { get; set; }

        public string PlaceId { get; set; } // Google Places place_id
        public string PlaceName { get; set; }
        public string PlaceAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public int? Capacity { get; set; } // NULL => unlimited

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public EventStatus Status { get; set; } = EventStatus.Published;

        public int CreditsSpent { get; set; } = 1;

        public virtual ICollection<EventParticipant> Participants { get; set; }
    }
}
