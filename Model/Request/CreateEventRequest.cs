using System;
using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Model.Request
{
    public class CreateEventRequest
    {
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public Guid InterestId { get; set; }
        
        [Required]
        public string PlaceId { get; set; }
        
        public string PlaceName { get; set; }
        public string PlaceAddress { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        
        [Required]
        public DateTime StartsAt { get; set; }
        
        [Required]
        public DateTime EndsAt { get; set; }
        
        public int? Capacity { get; set; }
    }
}
