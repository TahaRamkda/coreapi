using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TemplateAnalyticsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSolutionContext2 _dbcontext2;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ITemplateAnalyticsService _templateAnalyticsService;
        private readonly ILogger<AnalyticController> _logger;

        public TemplateAnalyticsController(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext2 _dbcontext2, WhatsAppSolutionContext dbContext, ITemplateAnalyticsService templateAnalyticsService, ILogger<AnalyticController> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            this._dbcontext2 = _dbcontext2;
            _dbContext = dbContext;
            _templateAnalyticsService = templateAnalyticsService;
            _logger = logger;
        }
        #region Method
        [AllowAnonymous]
        [HttpPost("GetTemplateAnalytic")]
        public async Task<IActionResult> GetTemplateAnalytic([FromBody] TemplateAnalyticsRequestDto model)
        {
            _logger.LogInformation("GetTemplateAnalytics called with model: {model}", JsonConvert.SerializeObject(model));
            var result = await _templateAnalyticsService.ProcessTemplateAnalyticsAsync(model);
            return Ok(result);
        }
        #endregion
    }
}
