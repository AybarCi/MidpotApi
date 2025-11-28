using DatingWeb.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class PlacesService : IPlacesService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PlacesService> _logger;
        private readonly string _apiKey;

        public PlacesService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<PlacesService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _apiKey = configuration["GooglePlaces:ApiKey"] ?? "";
        }

        public async Task<PlaceDetails> GetPlaceDetailsAsync(string placeId)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Places API key not configured. Skipping validation.");
                return null;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=name,formatted_address,geometry&key={_apiKey}";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places API returned error: {status}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var result = json.RootElement.GetProperty("result");

                return new PlaceDetails
                {
                    PlaceId = placeId,
                    Name = result.GetProperty("name").GetString(),
                    Address = result.GetProperty("formatted_address").GetString(),
                    Lat = result.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble(),
                    Lng = result.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching place details for placeId: {placeId}", placeId);
                return null;
            }
        }

        public async Task<IEnumerable<PlaceSearchResult>> SearchPlacesAsync(string query, double? lat, double? lng)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google Places API key not configured. Returning empty results.");
                return Enumerable.Empty<PlaceSearchResult>();
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var locationParam = lat.HasValue && lng.HasValue ? $"&location={lat},{lng}&radius=5000" : "";
                var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}{locationParam}&key={_apiKey}";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Google Places API search returned error: {status}", response.StatusCode);
                    return Enumerable.Empty<PlaceSearchResult>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(content);
                var results = json.RootElement.GetProperty("results");

                var places = new List<PlaceSearchResult>();
                foreach (var result in results.EnumerateArray())
                {
                    places.Add(new PlaceSearchResult
                    {
                        PlaceId = result.GetProperty("place_id").GetString(),
                        Name = result.GetProperty("name").GetString(),
                        Address = result.GetProperty("formatted_address").GetString(),
                        Lat = result.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble(),
                        Lng = result.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble()
                    });
                }

                return places;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching places for query: {query}", query);
                return Enumerable.Empty<PlaceSearchResult>();
            }
        }
    }
}
