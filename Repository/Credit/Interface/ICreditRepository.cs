using DatingWeb.Data.DbModel;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Credit.Interface
{
    public interface ICreditRepository
    {
        Task<int> GetUserBalanceAsync(long userId);
        Task AddTransactionAsync(CreditTransaction transaction);
    }
}
