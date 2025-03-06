using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.Model.Request;
using DatingWeb.Repository.Location.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class LocationController : BaseController
    {
        private readonly ILocationRepository _locationRepository;
        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        [HttpGet("get-location")]
        public async Task<IActionResult> GetLocation(int locationCode)
        {
            return Ok(await _locationRepository.GetLocation(locationCode));
        }
        [HttpGet("get-locations")]
        public async Task<IActionResult> GetLocations()
        {
            return Ok(await _locationRepository.GetLocations());
        }
        [HttpPost("add-location")]
        public async Task<IActionResult> AddLocation([FromBody] AddLocationRequest request)
        {
            return Ok(await _locationRepository.AddLocation(request));
        }
    }
}

