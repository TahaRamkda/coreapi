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

namespace WhatsAppAPISolutionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyticController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppSolutionContext2 _dbcontext2;
        private readonly WhatsAppSolutionContext _dbContext;

        public AnalyticController(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext2 _dbcontext2, WhatsAppSolutionContext dbContext)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            this._dbcontext2 = _dbcontext2;
            _dbContext = dbContext;
        }
        [HttpPost("GetAnalytic")]
        public async Task<IActionResult> GetConversationAnalytics([FromBody] ConversationAnalyticRequestDto model)
        {
            var bridgeEndpoint = $"/api/Analytics/ConversationAnalytics";
            var bridgeDto = new ConversationAnalyticBridgeRequestDto
            {
                ClientId = model.ClientId.ToString(),
                SenderId = model.SenderId.ToString(),
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            var requestJson = JsonConvert.SerializeObject(bridgeDto);
            var content = new StringContent(requestJson, null, "application/json");
            var bridgeResponse = await _httpClient.PostAsync(bridgeEndpoint, content);

            if (!bridgeResponse.IsSuccessStatusCode)
            {
                return Ok(new ApiResult
                {
                    Message = "Bridge API failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }

            var bridgeContent = await bridgeResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SyncResultDto>(bridgeContent);

            if (result != null && result.success && result.result != null)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var bridgeResults = JsonConvert.DeserializeObject<List<ConversationAnalyticBridgeResponeDto>>(data);
                if (!bridgeResults.Any())
                {
                    return Ok(new ApiResult
                    {
                        Success = true,
                        Message = "ConversationAnalytics Not Exists",
                        StatusCode = StatusCodes.Status200OK
                    });
                }
               var conversationEntities = bridgeResults.Select(item => new ConversationAnalytic
               {
                    Start = item.Start,
                    End = item.End,
                    StartUtc = item.StartDateUtc,
                    EndUtc = item.EndDateUtc,
                    ConversationCount = item.Conversation,
                    PhoneNumber = item.PhoneNumber,
                    Cost = item.Cost,
                    Category = item.ConversationCategory,
                    ClientId = model.ClientId,
                    SenderId = model.SenderId,
                    CreatedDate = DateTime.UtcNow,
                    ConversationType = item.ConversationType
               }).ToList();

                await _dbContext.ConversationAnalytics.AddRangeAsync(conversationEntities);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new ApiResult
            {
                Success = true,
                Message = "Analytics data processed and stored successfully.",
                StatusCode = StatusCodes.Status200OK
            });
        }

    }
}
