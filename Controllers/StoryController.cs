using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Story.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class StoryController : BaseController
    {
        private readonly IStoryRepository _storyRepository;
        public StoryController(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        [HttpPost("add-story-photo"), RequestSizeLimit(99999999999)]
        public async Task<ActionResult> AddStoryPhoto([FromForm] IFormFile file)
        {
            //if (file.ContentType != "image/jpeg" && file.ContentType != "image/png" && file.ContentType != "image/jpg")
            //{
            //    return BadRequest();
            //}
                

            if (file == null)
                return BadRequest();

            var response = await _storyRepository.AddStoryPhoto(this.GetUserId, file, this.GetProfilePhoto, this.GetPersonName);

            return Ok(response);
        }
        [HttpPost("get-stories")]
        public async Task<IActionResult> GetStories(List<GetStoriesRequest> request)
        {
            var response = await _storyRepository.GetStories(this.GetUserId, this.GetPersonName, this.GetProfilePhoto,request);

            return Ok(response);
        }
    }
}

