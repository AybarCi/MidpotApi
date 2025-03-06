using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
	public class Privacy
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public string LanguageCode { get; set; }
        public string ContentKey { get; set; }
        public string Content { get; set; }
    }
}

