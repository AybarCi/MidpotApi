using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using DatingWeb.Model.Response;
using System.Collections.Generic;
using DatingWeb.Model.Request;

namespace DatingWeb.Repository.Story.Interface
{
    public interface IStoryRepository
    {
        Task<List<AddStoryPhotoResponse>> AddStoryPhoto(long userId, IFormFile file, string profilePhoto, string userName);
        Task<List<GetStoriesResponse>> GetStories(long currentUser, string userName, string profilePhoto, List<GetStoriesRequest> getStoriesRequest);
    }
}

