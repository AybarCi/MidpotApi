using System;

namespace DatingWeb.Model.Response
{
    public class UserResponse
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public bool Gender { get; set; }
        public bool PreferredGender { get; set; }
        public DateTime BirthDate { get; set; }
        public string PersonName { get; set; }
        public bool GhostMode { get; set; }
    }
}
