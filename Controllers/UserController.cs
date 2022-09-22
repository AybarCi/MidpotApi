using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.User.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("get-profile")]
        public async Task<IActionResult> GetProfile(long model)
        {
            return Ok(await _userRepository.GetProfile(model));
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("UserId", this.GetUserId.ToString());
            dic.Add("PersonName", this.GetPersonName);
            dic.Add("BirthDate", this.GetBirthDate.ToString());
            dic.Add("Gender", this.GetGender.ToString());
            dic.Add("GreferredGender", this.GetPreferredGender.ToString());
            return Ok(dic);
        }

        [HttpPost("update-profile-settings")]
        public async Task<IActionResult> UpdateProfileSettings(UpdateProfileSettingsRequest model)
        {
            return Ok(await _userRepository.UpdateProfileSettings(this.GetUserId, model.Description, model.FromAge, model.UntilAge, model.School, model.Job, model.DeviceToken, model.GhostMode));
        }

        [HttpPost("update-profile-photo"), RequestSizeLimit(100000)]
        public async Task<ActionResult> UpdateProfilePhoto([FromForm] IFormFile file)
        {
            if (file.ContentType != "image/jpeg")
                return BadRequest();

            if (file == null)
                return BadRequest();

            //string fileName = Guid.NewGuid().ToString();
            //var response = await _blobService.UploadFileBlobAsync("firstcontainer", file.OpenReadStream(), file.ContentType, string.Format("{0}.jpeg", fileName));
            //await _userRepository.UpdateProfilePhoto(this.GetUserId, response.OriginalString);

            var response = await _userRepository.UpdateProfilePhotoNew(this.GetUserId, file);

            return Ok(new UpdateProfilePhotoResponse { ProfilePhoto = response });
        }

        [HttpGet("delete-user")]
        public async Task<ActionResult> DeleteUser()
        {
            return Ok(await _userRepository.DeleteUser(this.GetUserId));
        }
    }
}
