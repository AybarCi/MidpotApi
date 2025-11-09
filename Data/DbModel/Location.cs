using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingWeb.Data.DbModel
{
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string City { get; set; }
    }
}

