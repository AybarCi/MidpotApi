using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class CreditTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public int Change { get; set; } // positive for purchase, negative for spending
        public int BalanceAfter { get; set; }

        [Required]
        public string Type { get; set; } // 'purchase','spend','refund'

        public string Metadata { get; set; } // JSON string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
