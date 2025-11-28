using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class UserInterest
    {
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public Guid InterestId { get; set; }
        [ForeignKey("InterestId")]
        public virtual Interest Interest { get; set; }
    }
}
