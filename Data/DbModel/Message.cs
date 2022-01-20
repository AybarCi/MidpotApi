using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long MessageId { get; set; }
        public long MatchId { get; set; }
        public long UserId { get; set; }
        [MaxLength(144)]
        public string Chat { get; set; }
        public DateTime CreateDate { get; set; }
        public string IpAddress { get; set; }
        public bool IsRead { get; set; }
    }
}
