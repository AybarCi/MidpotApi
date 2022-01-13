using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class Report
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ReportId { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public long MatchId { get; set; }
        public long UserId { get; set; }
    }
}
