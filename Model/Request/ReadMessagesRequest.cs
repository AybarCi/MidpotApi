using System;
using System.Collections.Generic;

namespace DatingWeb.Model.Request
{
    public class ReadMessagesRequest
    {
        public List<long> MessageIds { get; set; }
    }
}

