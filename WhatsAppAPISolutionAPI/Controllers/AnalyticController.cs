using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Dto.BridgeResponseDto;
using Serilog.Sinks.Http;
using Microsoft.AspNetCore.Http;
using System.Net;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.Dto.Common;
using System.Text;
using WhatsAppAPISolutionDL.UserModels.Entity;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionBL.Master.Interfaces;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticController : ControllerBase
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSolutionContext2 _dbcontext2;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly IConversationAnalyticsService _conversationAnalyticsService;
        #endregion

        #region Ctor
        public AnalyticController(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext2 _dbcontext2, WhatsAppSolutionContext dbContext, IConversationAnalyticsService conversationAnalyticsService)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            this._dbcontext2 = _dbcontext2;
            _dbContext = dbContext;
            _conversationAnalyticsService = conversationAnalyticsService;
        }
        #endregion

        #region Method
        [HttpPost("GetAnalytic")]
        public async Task<IActionResult> GetConversationAnalytics([FromBody] ConversationAnalyticRequestDto model)
        {
            var result = await _conversationAnalyticsService.ProcessConversationAnalyticsAsync(model);
            return Ok(result);
        }
        #endregion
    }
}

