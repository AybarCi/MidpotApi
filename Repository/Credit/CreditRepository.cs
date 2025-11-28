using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Repository.Credit.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Credit
{
    public class CreditRepository : ICreditRepository
    {
        private readonly ApplicationDbContext _context;

        public CreditRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserBalanceAsync(long userId)
        {
            var hasTransactions = await _context.CreditTransactions.AnyAsync(t => t.UserId == userId);
            if (!hasTransactions) return 0;

            return await _context.CreditTransactions
                .Where(t => t.UserId == userId)
                .SumAsync(t => t.Change);
        }

        public async Task AddTransactionAsync(CreditTransaction transaction)
        {
            await _context.CreditTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
