using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;
using WhatsAppAPISolutionDL.Dto.Common;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Newtonsoft.Json.Linq;

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
        #endregion

        #region Ctor
        public TemplateAnalyticsController(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext2 _dbcontext2, WhatsAppSolutionContext dbContext, ITemplateAnalyticsService templateAnalyticsService, ILogger<AnalyticController> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            this._dbcontext2 = _dbcontext2;
            _dbContext = dbContext;
            _templateAnalyticsService = templateAnalyticsService;
            _logger = logger;
        }
        #endregion

        #region Method
        [AllowAnonymous]
        [HttpPost("GetTemplateAnalytic")]
        public async Task<IActionResult> GetTemplateAnalytic([FromBody] TemplateAnalyticsRequestDto model)
        {
            _logger.LogInformation("GetTemplateAnalytics called with model: {model}", JsonConvert.SerializeObject(model));
            var result = await _templateAnalyticsService.ProcessTemplateAnalyticsAsync(model);
            return Ok(result);
        }
        [HttpGet("GetTemplateList")]
        public async Task<IActionResult> GetTemplateAnalyticsList([FromQuery] int clientId,[FromQuery] int senderId,[FromQuery] int templateId,[FromQuery] DateTime? startDate,[FromQuery] DateTime? endDate)
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
        #endregion
    }
}
