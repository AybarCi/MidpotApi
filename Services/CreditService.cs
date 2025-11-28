using DatingWeb.Data.DbModel;
using DatingWeb.Repository.Credit.Interface;
using DatingWeb.Services.Interfaces;
using System.Threading.Tasks;

namespace DatingWeb.Services
{
    public class CreditService : ICreditService
    {
        private readonly ICreditRepository _creditRepository;

        public CreditService(ICreditRepository creditRepository)
        {
            _creditRepository = creditRepository;
        }

        public async Task<int> GetUserBalanceAsync(long userId)
        {
            return await _creditRepository.GetUserBalanceAsync(userId);
        }

        public async Task<bool> DeductCreditsAsync(long userId, int amount, string type, string metadata)
        {
            var currentBalance = await GetUserBalanceAsync(userId);
            if (currentBalance < amount) return false;

            var transaction = new CreditTransaction
            {
                UserId = userId,
                Change = -amount,
                BalanceAfter = currentBalance - amount,
                Type = type,
                Metadata = metadata
            };

            await _creditRepository.AddTransactionAsync(transaction);
            return true;
        }

        public async Task AddCreditsAsync(long userId, int amount, string type, string metadata)
        {
            var currentBalance = await GetUserBalanceAsync(userId);
            var transaction = new CreditTransaction
            {
                UserId = userId,
                Change = amount,
                BalanceAfter = currentBalance + amount,
                Type = type,
                Metadata = metadata
            };

            await _creditRepository.AddTransactionAsync(transaction);
        }
    }
}
