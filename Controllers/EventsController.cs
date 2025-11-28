using DatingWeb.Model.Request;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            try
            {
                var result = await _eventService.CreateEventAsync(userId, request);
                // CreatedAtAction requires GetEvent to be implemented, using Ok for now to avoid runtime error if GetEvent is missing logic
                return StatusCode(201, result); 
            }
            catch (InvalidOperationException ex)
            {
                // Return 402 Payment Required if insufficient credits (as per prompt)
                if (ex.Message.Contains("credits"))
                {
                    return StatusCode(402, new { message = ex.Message });
                }
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the event." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] Guid? interestId)
        {
            var events = await _eventService.GetLatestEventsAsync(interestId);
            return Ok(events);
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(Guid id)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {
                await _eventService.JoinEventAsync(id, userId);
                return Ok(new { status = "joined" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/invite")]
        public async Task<IActionResult> InviteUser(Guid id, [FromBody] InviteUserRequest request)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _eventService.InviteUserAsync(id, userId, request.UserId);
            return Ok(new { message = "invited" });
        }
    }
}
