using System;
namespace DatingWeb.Model.Request
{
    public class GetStoriesRequest
    {
        public long Id { get; set; }
        public string PersonName { get; set; }
        public string ProfilePhoto { get; set; }
    }
}

