using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using System.Threading.Tasks;

namespace DatingWeb.Repository.PremiumUsers.Interface
{
    public interface IPremiumUserRepository
    {
        Task<PremiumUserResponse> GetPremiumUser(long userId);
        Task<bool> RegisterPremiumUser(long userId, RegisterPremiumUserRequest model);
    }
}
