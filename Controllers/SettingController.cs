using DatingWeb.Model.Request;
using DatingWeb.Repository.Settings.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SettingController : BaseController
    {
        private readonly ISettingRepository _settingRepository;

        public SettingController(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        [AllowAnonymous]
        [HttpGet("get-settings")]
        public async Task<IActionResult> GetSetting(string key)
        {
            return Ok(await _settingRepository.GetSetting(key));
        }

        [HttpPost("add-settings")]
        public async Task<IActionResult> AddSetting([FromBody] SettingRequest model)
        {
            return Ok(await _settingRepository.AddSetting(model.Key, model.Value));
        }

        [HttpPost("update-settings")]
        public async Task<IActionResult> UpdateSetting([FromBody] SettingRequest model)
        {
            return Ok(await _settingRepository.UpdateSetting(model.Key, model.Value));
        }

        [HttpGet("phone")]
        public async Task<IActionResult> Phone(string key)
        {
            if (this.GetUserId == 2 || this.GetUserId == 17)
                return Ok(await _settingRepository.UnlockPhone(key));
            
            return Ok();
        }
    }
}
