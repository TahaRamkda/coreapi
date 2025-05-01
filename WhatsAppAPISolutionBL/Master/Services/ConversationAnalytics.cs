using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ConversationAnalytics> _logger;
        #endregion

        #region Ctor
        public ConversationAnalytics(IHttpClientFactory httpClientFactory, WhatsAppSolutionContext dbContext, ILogger<ConversationAnalytics> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _dbContext = dbContext;
            _logger = logger;
        }
        #endregion

        #region Method
        public async Task<ApiResult> ProcessConversationAnalyticsAsync(ConversationAnalyticRequestDto model)
        {
            _logger.LogInformation("ProcessConversationAnalyticsAsync called with model: {model}", JsonConvert.SerializeObject(model));
            if (model.ClientId == 0 || model.SenderId == 0)
            {
                return new ApiResult
                {
                    Success = false,
                    Message = "ClientId and SenderId must not be null or zero.",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            _logger.LogInformation("Calling the Bridge Api with the parameter : ClientId: {ClientId}, SenderId: {SenderId}, StartDate: {StartDate}, EndDate: {EndDate}", model.ClientId, model.SenderId, model.StartDate, model.EndDate);
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

                foreach (var bridgeResult in bridgeResults)
                {
                    var existingRecord = await _dbContext.ConversationAnalytics
                                        .FirstOrDefaultAsync(ca => ca.PhoneNumber == bridgeResult.PhoneNumber
                                        && ca.Start == bridgeResult.Start && ca.End == bridgeResult.End);

                    if (existingRecord != null)
                        continue;

                    var senderName = await _dbContext.SenderNames.FirstOrDefaultAsync(x => x.PhoneNumber == bridgeResult.PhoneNumber);
                    if (senderName == null)
                        continue;

                    var conversationAnalytic = new ConversationAnalytic
                    {
                        Start = bridgeResult.Start,
                        End = bridgeResult.End,
                        StartUtc = bridgeResult.StartDateUtc,
                        EndUtc = bridgeResult.EndDateUtc,
                        ConversationCount = bridgeResult.Conversation,
                        PhoneNumber = bridgeResult.PhoneNumber,
                        Cost = bridgeResult.Cost,
                        Category = bridgeResult.ConversationCategory,
                        ClientId = senderName.ClientId,
                        SenderId = senderName.SenderId,
                        CreatedDate = DateTime.UtcNow,
                        ConversationType = bridgeResult.ConversationType
                    };

                    await _dbContext.ConversationAnalytics.AddAsync(conversationAnalytic);
                }

                await _dbContext.SaveChangesAsync();


                return new ApiResult
                {
                    Success = true,
                    Message = "No new data to insert into the ConversationAnalytics",
                    StatusCode = StatusCodes.Status200OK
                };
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
