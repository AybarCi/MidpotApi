namespace DatingWeb.Model.Request
{
    public class NotificationMessageHubRequest
    {
        public Notification notification { get; set; }
        public MessageDataHub data { get; set; }
        public string to { get; set; }
    }

    public class MessageDataHub
    {
        public string matchId { get; set; }
    }
}
