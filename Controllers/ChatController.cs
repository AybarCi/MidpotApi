using DatingWeb.Model.Request;
using DatingWeb.Repository.Chat.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        private readonly IChatRepository _chatRepository;
        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        [HttpPost("load-conversation")]
        public async Task<IActionResult> LoadConversation([FromBody] LoadConversationRequest model)
        {
            return Ok(await _chatRepository.LoadConversation(this.GetUserId, model.MatchId, model.LastUpdateId));
        }
        [HttpPost("get-new-messages")]
        public async Task<IActionResult> GetNewMessages([FromBody] GetNewMessagesRequest request)
        {
            return Ok(await _chatRepository.GetNewMessages(this.GetUserId, request.LastSawMessagesDate));     
        }
    }
}
