using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Auth.Interface
{
    public interface IAuthRepository
    {
        Task Register(string personName, string password, string phoneNumber, string birthDate, bool gender, bool preferredGender, string deviceToken, bool platform, int fromAge, int untilAge, string school, string job);
        Task<TokenResponse> Login(string phoneNumber, string password, string deviceToken, bool platform);
        Task<bool> LogOut(long userId);
        Task<TokenResponse> RefreshToken(string refreshToken);
        Task<TokenResponse> ConfirmPhone(string phoneNumber, string confirmCode);
        Task<string> RequestCodeAgain(string phoneNumber);
        Task<bool> ForgotPassword(string phoneNumber);
        Task<TokenResponse> ForgotPasswordConfirm(string phoneNumber, string confirmCode, string password);
        Task<bool> ChangePassword(long userId, string password, string newPassword);
    }
}
