using System.Threading.Tasks;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Privacy.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrivacyController : BaseController
    {
        private readonly IPrivacyRepository _privacyRepository;
        public PrivacyController(IPrivacyRepository privacyRepository)
        {
            _privacyRepository = privacyRepository;
        }
        [HttpGet("get-content")]
        public async Task<IActionResult> Get(string languageCode, string contentKey)
        {
            return Ok(await _privacyRepository.GetContent(languageCode, contentKey));
        }
    }
}

