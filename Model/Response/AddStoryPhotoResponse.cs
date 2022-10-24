using System;
using System.Collections.Generic;

namespace DatingWeb.Model.Response
{
    public class AddStoryPhotoResponse
    {
        public AddStoryPhotoResponse()
        {
            this.stories = new List<UserStory>();
        }
        public long user_id { get; set; }
        public string user_name { get; set; }
        public string user_image { get; set; }
        public bool current_user { get; set; }
        public int story_count { get; set; }
        public List<UserStory> stories { get; set; }
        public bool is_added_story { get; set; }
    }
}

