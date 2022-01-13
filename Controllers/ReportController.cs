using DatingWeb.Model.Request;
using DatingWeb.Repository.Report.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DatingWeb.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportRepository _reportRepository;
        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpPost("send-report")]
        public async Task<IActionResult> SendReport([FromBody] ReportRequest model)
        {
            return Ok(await _reportRepository.SendReport(this.GetUserId, model.MatchId, model.Description));
        }
    }
}
