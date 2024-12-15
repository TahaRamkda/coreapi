using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
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
        public async Task<ActionResult> GetDashboardSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var res = await _dashboardService.GetDashboardSummaryListAsync(ClientId, DashboardTypeId, FromDate, ToDate);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getdashboardreportsummary")]
        public async Task<ActionResult> GetDashboardReportSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var res = await _dashboardService.GetDashboardReportSummaryListAsync(ClientId, DashboardTypeId, FromDate, ToDate);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
