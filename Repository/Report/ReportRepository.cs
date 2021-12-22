using DatingWeb.Data;
using DatingWeb.Repository.Report.Interface;
using System;
using System.Threading.Tasks;

namespace DatingWeb.Repository.Report
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
        }

        public async Task<bool> SendReport(long userId, long matchId, string description)
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            await _context.Report.AddAsync(new Data.DbModel.Report { UserId = userId, MatchId = matchId, Description = description });
            var val = await _context.SaveChangesAsync();
            return val > 0 ? true : false;
        }
    }
}
