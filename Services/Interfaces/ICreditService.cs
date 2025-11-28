using System.Threading.Tasks;

namespace DatingWeb.Services.Interfaces
{
    public interface ICreditService
    {
        Task<int> GetUserBalanceAsync(long userId);
        Task<bool> DeductCreditsAsync(long userId, int amount, string type, string metadata);
        Task AddCreditsAsync(long userId, int amount, string type, string metadata);
    }
}
