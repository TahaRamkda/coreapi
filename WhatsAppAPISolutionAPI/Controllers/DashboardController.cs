using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService,
            WhatsAppSolutionContext dbContext,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getdashboardsummary")]
        public async Task<ActionResult> GetDashboardSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var res = await _dashboardService.GetDashboardSummaryListAsync(client_Id, dashboardType_Id, fromDate, toDate);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getdashboardreportsummary")]
        public async Task<ActionResult> GetDashboardReportSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var res = await _dashboardService.GetDashboardReportSummaryListAsync(client_Id, dashboardType_Id, fromDate, toDate);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
