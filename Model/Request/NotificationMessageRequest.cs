using System;

namespace DatingWeb.Model.Request
{
    public class Notification
    {
        public string title { get; set; }
        public string body { get; set; }
    }

    public class MessageData
    {
        public string conversationId { get; set; }
        public string chat { get; set; }
        public string userId { get; set; }
        public string createDate { get; set; }
    }

    public class NotificationMessageRequest
    {
        public Notification notification { get; set; }
        public MessageData data { get; set; }
        public string to { get; set; }
    }
}
