using DatingWeb.Repository.Interest.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterestsController : ControllerBase
    {
        private readonly IInterestRepository _interestRepository;

        public InterestsController(IInterestRepository interestRepository)
        {
            _interestRepository = interestRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetInterests()
        {
            var interests = await _interestRepository.GetAllInterestsAsync();
            return Ok(interests);
        }
    }
}
