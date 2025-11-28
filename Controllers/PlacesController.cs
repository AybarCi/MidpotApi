using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly IPlacesService _placesService;

        public PlacesController(IPlacesService placesService)
        {
            _placesService = placesService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPlaces([FromQuery] string q, [FromQuery] double? lat, [FromQuery] double? lng)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Query parameter 'q' is required");
            }

            var results = await _placesService.SearchPlacesAsync(q, lat, lng);
            return Ok(results);
        }

        [HttpGet("{placeId}")]
        public async Task<IActionResult> GetPlaceDetails(string placeId)
        {
            var details = await _placesService.GetPlaceDetailsAsync(placeId);
            if (details == null)
            {
                return NotFound("Place not found");
            }
            return Ok(details);
        }
    }
}
