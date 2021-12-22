using System.Threading.Tasks;

namespace DatingWeb.Repository.Report.Interface
{
    public interface IReportRepository
    {
        Task<bool> SendReport(long userId, long matchId, string description);
    }
}
