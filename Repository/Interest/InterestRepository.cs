using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Repository.Interest.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Interest
{
    public class InterestRepository : IInterestRepository
    {
        private readonly ApplicationDbContext _context;

        public InterestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DatingWeb.Data.DbModel.Interest>> GetAllInterestsAsync()
        {
            return await _context.Interests.ToListAsync();
        }

        public async Task<DatingWeb.Data.DbModel.Interest> GetInterestByIdAsync(Guid id)
        {
            return await _context.Interests.FindAsync(id);
        }
    }
}
