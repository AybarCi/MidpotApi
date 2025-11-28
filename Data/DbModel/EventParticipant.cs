using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class EventParticipant
    {
        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } // 'invited','joined','declined','no_show'

        public int? RatingByCreator { get; set; } // 1..5
        public int? RatingByUser { get; set; } // 1..5
        
        // Flag to track if user checked in (to avoid no-show)
        public bool CheckedIn { get; set; } = false;
    }
}
