using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Model.Request
{
    public class UserRegisterRequest
    {
        public string PersonName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthDate { get; set; }
        public bool Gender { get; set; }
        public bool PreferredGender { get; set; }
        public string DeviceToken { get; set; }
        public bool Platform { get; set; }
        public int FromAge { get; set; }
        public int UntilAge { get; set; }
        public string School { get; set; }
        public string Job { get; set; }
    }
}
