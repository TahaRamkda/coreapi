using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Globalization;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly int clientId;
        private readonly IDashboardService _dashboardService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<DashboardController> _logger;
        private readonly IUserService _userService;

        public DashboardController(IDashboardService dashboardService,
            WhatsAppSolutionContext dbContext,
            ILogger<DashboardController> logger,
            IUserService userService)
        {
            _dashboardService = dashboardService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getdashboardsummary")]
        public async Task<ActionResult> GetDashboardSummaryAsync(int senderId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            _logger.LogInformation("Calling api GetDashboardSummaryAsync with clientId={clientId}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}", clientId, senderId, fromDate, toDate);

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            if (!fromDate.HasValue && !toDate.HasValue)
                return Ok(new ApiResult { Message = "Date range is required" });

            var res = await _dashboardService.GetDashboardSummaryAsync(clientId, senderId, fromDate, toDate);
            if (String.IsNullOrWhiteSpace(res))
                return Ok(new ApiResult { Message = "No data found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplateinsight")]
        public async Task<ActionResult> GetTemplateInsightAsync(int templateId = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            _logger.LogInformation("Calling api GetTemplateInsightAsync with clientId={clientId}, templateId={templateId}, fromDate={fromDate}, toDate={toDate}", clientId, templateId, fromDate, toDate);

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            if (!fromDate.HasValue && !toDate.HasValue)
                return Ok(new ApiResult { Message = "Date range is required" });

            if (templateId == 0)
                return Ok(new ApiResult { Message = "Template id is required" });

            var res = await _dashboardService.GetTemplateInsightAsync(clientId, templateId, fromDate, toDate);
            if (String.IsNullOrWhiteSpace(res))
                return Ok(new ApiResult { Message = "No data found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
