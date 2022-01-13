using System;

namespace DatingWeb.Model.Response
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public long UserId { get; set; }
        public int BirthDate { get; set; }
        public string PersonName { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public string Description { get; set; }
        public string ProfilePhoto { get; set; }
        public bool Gender { get; set; }
        public bool PreferredGender { get; set; }
        public int FromAge { get; set; }
        public int UntilAge { get; set; }
        public string School { get; set; }
        public string Job { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
