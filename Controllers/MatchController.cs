using DatingWeb.Model.Request;
using DatingWeb.Repository.Matches.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MatchController : BaseController
    {
        private readonly IMatchRepository _matchRepository;
        public MatchController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        [HttpPost("get-matched")]
        public async Task<IActionResult> GetMatched([FromBody] GetMatchRequest model)
        {
            return Ok(await _matchRepository.MatchMachine(this.GetUserId, this.GetGender, this.GetPreferredGender, model.LastMatchDate, model.Latitude, model.Longitude));
        }

        [HttpPost("remove-match")]
        public async Task<IActionResult> RemoveMatch([FromBody] RemoveMatchRequest input)
        {
            return Ok(await _matchRepository.RemoveMatch(this.GetUserId, input.MatchId));
        }
    }
}
