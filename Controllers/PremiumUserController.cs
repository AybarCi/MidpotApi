using DatingWeb.Model.Request;
using DatingWeb.Repository.PremiumUsers.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PremiumUserController : BaseController
    {
        private readonly IPremiumUserRepository _premiumUserRepository;

        public PremiumUserController(IPremiumUserRepository premiumUserRepository)
        {
            _premiumUserRepository = premiumUserRepository;
        }

        [HttpGet("get-premium-user")]
        public async Task<IActionResult> GetPremiumUser()
        {
            return Ok(await _premiumUserRepository.GetPremiumUser(this.GetUserId));
        }
        [HttpGet("get-premium-users")]
        public async Task<IActionResult> GetPremiumUsers()
        {
            return Ok(await _premiumUserRepository.GetPremiumUsers());
        }
        [HttpGet("get-premium-users-count")]
        public async Task<IActionResult> GetPremiumUsersCount()
        {
            return Ok(await _premiumUserRepository.GetPremiumUsersCount());
        }
        [HttpPost("register-premium-user")]
        public async Task<IActionResult> RegisterPremiumUser([FromBody] RegisterPremiumUserRequest model)
        {
            return Ok(await _premiumUserRepository.RegisterPremiumUser(this.GetUserId, model));
        }
    }
}
