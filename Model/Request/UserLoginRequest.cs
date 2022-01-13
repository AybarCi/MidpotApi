using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Model.Request
{
    public class UserLoginRequest
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public bool Platform { get; set; }
    }
}
