using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class Story
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long StoryId { get; set; }
        public long UserId { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

