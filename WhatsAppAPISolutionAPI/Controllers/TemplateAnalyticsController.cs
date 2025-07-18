using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplateAnalyticsController : Controller
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSolutionContext2 _dbcontext2;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ITemplateAnalyticsService _templateAnalyticsService;
        private readonly ILogger<AnalyticController> _logger;
        private readonly int clientId;
        private readonly IUserService _userService;
        private readonly IExportManager _exportManager;
        #endregion

        #region Ctor
        public TemplateAnalyticsController(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext2 _dbcontext2, WhatsAppSolutionContext dbContext, ITemplateAnalyticsService templateAnalyticsService, ILogger<AnalyticController> logger, IUserService userService, IExportManager exportManager)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            this._dbcontext2 = _dbcontext2;
            _dbContext = dbContext;
            _templateAnalyticsService = templateAnalyticsService;
            _logger = logger;
            _userService = userService;
            clientId = _userService.GetClientIdFromAccessToken();
            _exportManager = exportManager;
        }
        #endregion

        #region Method
        [AllowAnonymous]
        [HttpPost("gettemplateanalytic")]
        public async Task<IActionResult> GetTemplateAnalytic([FromBody] TemplateAnalyticsRequestDto model)
        {
            _logger.LogInformation("GetTemplateAnalytics called with model: {model}", JsonConvert.SerializeObject(model));
            var result = await _templateAnalyticsService.ProcessTemplateAnalyticsAsync(model);
            return Ok(result);
        }
        [HttpGet("gettemplatelist")]
        public async Task<IActionResult> GetTemplateAnalyticsList(int clientId, [FromQuery] int senderId,[FromQuery] string templateId,[FromQuery] DateTime? startDate,[FromQuery] DateTime? endDate)
        {
            _logger.LogInformation("GetTemplateAnalyticsList called with clientId: {clientId}, senderId : {senderId}, templateId : {templateId}, startDate : {startDate}, endDate : {endDate}", clientId, senderId, templateId, startDate, endDate);
            var result = await _templateAnalyticsService.GetAnalyticsSummaryAsync(clientId, senderId, templateId, startDate, endDate);
            return Ok(new ApiResult
            {
                Result = result,
                Success = true,
                Message = "Template analytics summary retrieved successfully.",
            });
        }
        [HttpGet("gettemplateanalyticdetails")]
        public async Task<IActionResult> GetTemplateAnalyticsDetailsList([FromQuery] int senderId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string templateId = null)
        {
            _logger.LogInformation("GetTemplateAnalyticsDetailsList called with clientId: {clientId}, senderId : {senderId}, templateId : {templateId}, startDate : {startDate}, endDate : {endDate}", clientId, senderId, templateId, startDate, endDate);
            var result = await _templateAnalyticsService.GetAnalyticsDetailsList(clientId, senderId, startDate, endDate, templateId);
            return Ok(new ApiResult
            {
                Result = result,
                Success = true,
                Message = "Template analytics summary retrieved successfully.",
            });
        }
            [HttpGet("exporttemplatetnalyticssummaryreport")]
            public async Task<IActionResult> ExportTemplateAnalyticsDetails([FromQuery] int senderId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string templateId = null)
            {
                _logger.LogInformation("GetTemplateAnalyticsDetailsList called with clientId: {clientId}, senderId : {senderId}, templateId : {templateId}, startDate : {startDate}, endDate : {endDate}", clientId, senderId, templateId, startDate, endDate);
                var result = await _templateAnalyticsService.GetExportAnalyticsDetailsList(clientId, senderId, startDate, endDate, templateId);
                return Ok(new ApiResult {
                    Result = result,
                    Success = true,
                    Message = "Template Details"
                });
        }


        #endregion
    }
}
