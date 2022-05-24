using System;
using System.Collections.Generic;

namespace DatingWeb.Model.Request
{
    public class ReadMessagesRequest
    {
        public long MatchId { get; set; }
        public long MatchedUserId { get; set; }
    }
}

