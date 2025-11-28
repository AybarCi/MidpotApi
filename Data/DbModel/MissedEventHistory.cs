using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class MissedEventHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public Guid EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
