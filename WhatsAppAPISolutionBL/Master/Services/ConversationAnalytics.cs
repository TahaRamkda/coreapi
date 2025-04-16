using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;


namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ConversationAnalytics : IConversationAnalyticsService
    {
        #region Fields
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly HttpClient _httpClient;
        #endregion

        #region Ctor
        public ConversationAnalytics(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext dbContext)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _dbContext = dbContext;
        }
        #endregion

        #region Method
        public async Task<ApiResult> ProcessConversationAnalyticsAsync(ConversationAnalyticRequestDto model)
        {
            if (model.ClientId == 0 || model.SenderId == 0)
            {
                return new ApiResult
                {
                    Success = false,
                    Message = "ClientId and SenderId must not be null or zero.",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            var bridgeEndpoint = "/api/Analytics/ConversationAnalytics";
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
                return new ApiResult
                {
                    Message = "Bridge API failed",
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var bridgeContent = await bridgeResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SyncResultDto>(bridgeContent);

            if (result != null && result.success && result.result != null)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var bridgeResults = JsonConvert.DeserializeObject<List<ConversationAnalyticBridgeResponeDto>>(data);
                if (!bridgeResults.Any())
                {
                    return new ApiResult
                    {
                        Success = true,
                        Message = "ConversationAnalytics Not Exists",
                        StatusCode = StatusCodes.Status200OK
                    };
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

            return new ApiResult
            {
                Success = true,
                Message = "Analytics data processed and stored successfully.",
                StatusCode = StatusCodes.Status200OK
            };
        }
        #endregion
    }
}
