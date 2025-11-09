using System;
namespace DatingWeb.Model.Response
{
    public class LocationResponse
    {
        public long LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string City { get; set; }
    }
}

