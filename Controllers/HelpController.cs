using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class HelpController : BaseController
    {
        [Produces("application/json")]
        [Route("/")]
        [HttpGet]
        public IActionResult Root()
        {
            var deployment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return Ok($"V:{deployment} Warmed up successfully {DateTime.UtcNow}");
        }

        [HttpGet("version")]
        public IActionResult Version()
        {
            return Ok(new { v = "1.1" });
        }

        [HttpGet("chat")]
        public IActionResult Chat()
        {
            //var chat = Environment.GetEnvironmentVariable("CHAT");
            return Ok(new { ipaddress = "https://api.midpot.app" });
        }
    }
}
