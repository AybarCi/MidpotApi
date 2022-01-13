using System;
namespace DatingWeb.Model.Response
{
    public class NewMessagesResponse
    {
        public long UserId { get; set; }
        public int MessageCount { get; set; }
        public bool IsRead { get; set; }
    }
}

