using System;

namespace DatingWeb.Model.Response
{
    public class MessageResponse
    {
        public long UserId { get; set; }
        public string Chat { get; set; }
        public DateTime CreateDate { get; set; }
        public long MessageId { get; set; }
    }
}
