using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.PremiumUsers.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWeb.Repository.PremiumUsers
{
    public class PremiumUserRepository : IPremiumUserRepository
    {
        private readonly ApplicationDbContext _context;

        public PremiumUserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
        }

        /// <summary>
        /// GetPremiumUser
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PremiumUserResponse> GetPremiumUser(long userId)
        {
            return await _context.PremiumUser.Where(x => x.UserId == userId).Select(x => new PremiumUserResponse
            {
                UserId = x.UserId,
                PurchaseDate = x.PurchaseDate,
                ExpiresDate = x.ExpiresDate,
                ProductId = x.ProductId,
                TransactionId = x.TransactionId
            }).OrderByDescending(x => x.ProductId).FirstOrDefaultAsync();
        }

        public async Task<bool> RegisterPremiumUser(long userId, RegisterPremiumUserRequest model)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await _context.PremiumUser.AddAsync(new PremiumUser { UserId = userId, PurchaseDate = model.PurchaseDate, ExpiresDate = model.ExpiresDate, ProductId = model.ProductId, TransactionId = model.TransactionId });
            int val = await _context.SaveChangesAsync();
            return val == 1 ? true : false;
        }
    }
}
