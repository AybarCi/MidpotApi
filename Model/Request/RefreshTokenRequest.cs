using System.ComponentModel.DataAnnotations;

namespace DatingWeb.Model.Request
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token boş geçilemez!")]
        public string RefreshToken { get; set; }
    }
}
