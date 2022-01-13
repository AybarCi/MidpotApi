using System;

namespace DatingWeb.Model.Response
{
    public class MatchResponse
    {
        public long Id { get; set; }
        public long MatchId { get; set; }
        public string PersonName { get; set; }
        public DateTime BirthDate { get; set; }
        public string ProfilePhoto { get; set; }
        public string Description { get; set; }
        public string School { get; set; }
        public string Job { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
