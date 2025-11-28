using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface IPlacesService
    {
        Task<PlaceDetails> GetPlaceDetailsAsync(string placeId);
        Task<IEnumerable<PlaceSearchResult>> SearchPlacesAsync(string query, double? lat, double? lng);
    }

    public class PlaceDetails
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class PlaceSearchResult
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
