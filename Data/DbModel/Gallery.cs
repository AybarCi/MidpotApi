using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Data.DbModel
{
    public class Gallery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int GalleryId { get; set; }
        public long UserId { get; set; }
        public string Url { get; set; }
        public bool IsDelete { get; set; }
    }
}
