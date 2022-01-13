using System;

namespace DatingWeb.Model.Response
{
    public class ConversationResponse
    {
        public Guid ConversationId { get; set; }
        public long UserId { get; set; }
        public string PersonName { get; set; }
        public int BirthDate { get; set; }
        public string ProfilePhoto { get; set; }
    }
}
