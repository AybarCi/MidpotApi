using System;

namespace DatingWeb.Model.Request
{
    public class LoadConversationRequest
    {
        public long MatchId { get; set; }
        public long LastUpdateId { get; set; }
    }
}
