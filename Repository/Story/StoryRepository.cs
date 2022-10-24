using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Extension;
using DatingWeb.Model.Response;
using DatingWeb.Repository.Story.Interface;
using DatingWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingWeb.Model.Request;

namespace DatingWeb.Repository.Story
{
    public class StoryRepository : IStoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;
        private readonly ICache _cache;

        public StoryRepository(ApplicationDbContext context, IBlobService blobService, ICache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
            _blobService = blobService;
            _cache = cache;
        }
        public async Task<List<AddStoryPhotoResponse>> AddStoryPhoto(long userId, IFormFile file, string profilePhoto, string userName)
        {
            string fileName = Guid.NewGuid().ToString();

            await _blobService.UploadFileBlobAsync("secondcontainer", file.OpenReadStream(), file.ContentType, string.Format("{0}.jpeg", fileName));

            var story = new Data.DbModel.Story
            {
                PhotoUrl = fileName.ToStoryPhoto(),
                UserId = userId,
                CreateDate = DateTime.Now
            };
            await _context.Story.AddAsync(story);
            await _context.SaveChangesAsync();

            var stories = await _context.Story.Where(x => x.UserId == userId && x.CreateDate.AddDays(1) > DateTime.Now).ToListAsync();
            var storyList = new List<UserStory>();
            foreach (var item in stories)
            {
                storyList.Add(new UserStory { story_id = item.StoryId, story_image = item.PhotoUrl, swipeText = "" });
            }
            List<AddStoryPhotoResponse> addStories = new List<AddStoryPhotoResponse>();
            addStories.Add(new AddStoryPhotoResponse
            {
                current_user = true,
                story_count = 1,
                stories = storyList,
                user_id = userId,
                user_image = profilePhoto,
                user_name = userName,
                is_added_story = true,
            });
            
            return addStories;
        }

        public async Task<List<GetStoriesResponse>> GetStories(long currentUser,string userName,string profilePhoto,List<GetStoriesRequest> getStoriesRequest)
        {
            List<GetStoriesResponse> getStoriesResponse = new List<GetStoriesResponse>();
           
            var currentStories = await _context.Story.Where(x => x.UserId == currentUser && x.CreateDate.AddDays(1) > DateTime.Now).ToListAsync();
            if (currentStories.Count > 0)
            {
                List<UserStory> userStories = new List<UserStory>();
                foreach (var currentStory in currentStories)
                {
                    userStories.Add(new UserStory { story_id = currentStory.UserId, story_image = currentStory.PhotoUrl });
                }
                getStoriesResponse.Add(new GetStoriesResponse
                {
                    user_id = currentUser,
                    user_name = userName,
                    user_image = profilePhoto,
                    current_user = true,
                    story_count = currentStories.Count,
                    stories = userStories,
                    is_added_story = false,
                });
            } else
            {
                List<UserStory> userStories = new List<UserStory>();
                userStories.Add(new UserStory { story_id = currentUser, story_image = "", swipeText = "Custom swipe text for this story" });
                getStoriesResponse.Add(new GetStoriesResponse
                {
                    user_id = currentUser,
                    user_name = userName,
                    user_image = profilePhoto,
                    current_user = true,
                    story_count = 0,
                    stories = userStories,
                    is_added_story = false,
                });
            }
            if (getStoriesRequest.Count > 0)
            {
                foreach (var item in getStoriesRequest)
                {
                    List<UserStory> userStories = new List<UserStory>();
                    try
                    {
                        var stories = await _context.Story.Where(x => x.UserId == item.Id && x.CreateDate.AddDays(1) > DateTime.Now).ToListAsync();
                        if (stories.Count > 0)
                        {
                            foreach (var story in stories)
                            {
                                userStories.Add(new UserStory { story_id = story.StoryId, story_image = story.PhotoUrl });
                            }
                            getStoriesResponse.Add(new GetStoriesResponse
                            {
                                user_id = item.Id,
                                user_image = item.ProfilePhoto,
                                user_name = item.PersonName,
                                stories = userStories,
                                is_added_story = false,
                                current_user = false,
                                story_count = stories.Count
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            return getStoriesResponse;
        }
    }
}

