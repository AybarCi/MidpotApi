using DatingWeb.Model.Request;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CreditsController : ControllerBase
    {
        private readonly ICreditService _creditService;

        public CreditsController(ICreditService creditService)
        {
            _creditService = creditService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var balance = await _creditService.GetUserBalanceAsync(userId);
            return Ok(new { balance });
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseCredits([FromBody] PurchaseCreditsRequest request)
        {
            // Simulate purchase logic
            // In real world, this would create an order and return checkout URL
            // For now, we'll just add credits directly for testing (as per prompt "simulate")
            
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int credits = 0;
            
            switch(request.ProductId)
            {
                case "credits_5": credits = 5; break;
                case "credits_10": credits = 10; break;
                case "credits_20": credits = 20; break;
                default: return BadRequest("Invalid product id");
            }

            await _creditService.AddCreditsAsync(userId, credits, "purchase", "{\"productId\": \"" + request.ProductId + "\"}");
            
            return Ok(new { message = "Purchase successful", creditsAdded = credits });
        }
    }
}
