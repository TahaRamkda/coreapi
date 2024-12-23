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
        public async Task<ActionResult> GetDashboardSummaryAsync(int clientId, int senderId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                return Ok(new ApiResult
                {
                    Message = "Date range is required"
                });
            }

            var res = await _dashboardService.GetDashboardSummaryAsync(clientId, senderId, fromDate, toDate);
            if (String.IsNullOrWhiteSpace(res))
                return Ok(new ApiResult
                {
                    Message = "No data found"
                });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplateinsight")]
        public async Task<ActionResult> GetTemplateInsightAsync(int clientId, int templateId = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!fromDate.HasValue && !toDate.HasValue)
            {
                return Ok(new ApiResult
                {
                    Message = "Date range is required"
                });
            }

            if (templateId == 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Template id is required"
                });
            }

            var res = await _dashboardService.GetTemplateInsightAsync(clientId, templateId, fromDate, toDate);
            if (String.IsNullOrWhiteSpace(res))
                return Ok(new ApiResult
                {
                    Message = "No data found"
                });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
