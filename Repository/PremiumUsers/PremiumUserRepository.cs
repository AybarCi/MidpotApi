using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Model.Request;
using DatingWeb.Model.Response;
using DatingWeb.Repository.PremiumUsers.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            return await (from pu in _context.PremiumUser
                          join u in _context.ApplicationUsers on pu.UserId equals u.Id into puu
                          from premiumUser in puu.DefaultIfEmpty()
                          where pu.UserId == userId
                          select new PremiumUserResponse
                          {
                              ExpiresDate = pu.ExpiresDate,
                              ProductId = pu.ProductId,
                              PurchaseDate = pu.PurchaseDate,
                              TransactionId = pu.TransactionId,
                              UserId = pu.UserId,
                              UserName = premiumUser.PersonName
                          }
                          ).FirstOrDefaultAsync();
        }
        public async Task<List<PremiumUserResponse>> GetPremiumUsers()
        {
            return  await (from pu in _context.PremiumUser
                    join u in _context.ApplicationUsers on pu.UserId equals u.Id into puu
                    from premiumUser in puu.DefaultIfEmpty()
                    where pu.UserId != 17
                    orderby pu.PurchaseDate descending
                    select new PremiumUserResponse
                    {
                        ExpiresDate = pu.ExpiresDate,
                        ProductId = pu.ProductId,
                        PurchaseDate = pu.PurchaseDate,
                        TransactionId = pu.TransactionId,
                        UserId = pu.UserId,
                        UserName = premiumUser.PersonName,
                        ProfilePhoto = premiumUser.ProfilePhoto
                    }).ToListAsync();
        }
        public async Task<int> GetPremiumUsersCount()
        {
            return await _context.PremiumUser.Where(x => x.UserId != 17).CountAsync();
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
