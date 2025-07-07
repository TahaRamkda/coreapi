using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class TemplateAnalyticsService : ITemplateAnalyticsService
    {
        #region Fields

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConversationAnalyticsService> _logger;
        private readonly ISenderNameService _senderNameService;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        #endregion

        public TemplateAnalyticsService(IHttpClientFactory httpClientFactory,
          WhatsAppSolutionContext dbContext,
          WhatsAppSolutionContext2 dbContext2,
          ILogger<ConversationAnalyticsService> logger,
          ISenderNameService senderNameService)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _senderNameService = senderNameService;
        }

        public async Task<ApiResult> ProcessTemplateAnalyticsAsync(TemplateAnalyticsRequestDto model)
        {
            _logger.LogInformation("ProcessTemplateAnalyticsAsync called with model: {model}", JsonConvert.SerializeObject(model));
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
            var bridgeEndpoint = "/api/TemplateAnalytics/TemplateAnalytics";
            var bridgeDto = new TemplateAnalyticBridgeRequestDto
            {
                ClientId = model.ClientId.ToString(),
                SenderId = model.SenderId.ToString(),
                TemplateId = model.TemplateId.ToString(),
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
                var bridgeResults = JsonConvert.DeserializeObject<TemplateAnalyticsBridgeResponseDto>(data);
                if (bridgeResults == null)
                {
                    return new ApiResult
                    {
                        Success = true,
                        Message = "ConversationAnalytics Not Exists",
                        StatusCode = StatusCodes.Status200OK
                    };
                }

                //foreach (var bridgeResult in bridgeResults)
                //{
                //    var existingRecord = await _dbContext.ConversationAnalytics
                //                        .FirstOrDefaultAsync(ca => ca.PhoneNumber == bridgeResult.PhoneNumber
                //                        && ca.Start == bridgeResult.Start && ca.End == bridgeResult.End);

                //    if (existingRecord != null)
                //        continue;

                //    var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberAsync(bridgeResult.PhoneNumber);
                //    if (senderName == null)
                //        continue;

                //    var conversationAnalytic = new ConversationAnalytic
                //    {
                //        Start = bridgeResult.Start,
                //        End = bridgeResult.End,
                //        StartUtc = bridgeResult.StartDateUtc,
                //        EndUtc = bridgeResult.EndDateUtc,
                //        ConversationCount = bridgeResult.Conversation,
                //        PhoneNumber = bridgeResult.PhoneNumber,
                //        Cost = bridgeResult.Cost,
                //        Category = bridgeResult.ConversationCategory,
                //        ClientId = senderName.ClientId,
                //        SenderId = senderName.SenderId,
                //        CreatedDate = DateTime.UtcNow,
                //        ConversationType = bridgeResult.ConversationType
                //    };

                //    await _dbContext.ConversationAnalytics.AddAsync(conversationAnalytic);
                //}

                //await _dbContext.SaveChangesAsync();


                return new ApiResult
                {
                    Success = true,
                    Message = "Template Analytics Fetched successsfully",
                    StatusCode = StatusCodes.Status200OK,
                    Result = bridgeResults
                };
            }

            return new ApiResult
            {
                Success = true,
                Message = "Analytics data processed and stored successfully.",
                StatusCode = StatusCodes.Status200OK
            };


        }
        public async Task<List<UTemplateAnalyticsSummary>> GetAnalyticsSummaryAsync(int clientId,int senderId, string templateId,DateTime? startDate,DateTime? endDate)
        {
            var result = await _dbContext2.TemplateAnalyticsSummary.FromSqlInterpolated($"exec usp_TemplateAnalytics_Ops @ActionId={(int)CrudEnum.List},@ClientId = {clientId},@SenderId = {senderId}, @TemplateId = {templateId},@StartDate = {startDate},@EndDate = {endDate}").ToListAsync();
            return result;
        }
        public async Task<List<UTemplateAnalyticsDetailsList>> GetAnalyticsDetailsList(int? clientId,int? senderId, DateTime? startDate, DateTime? endDate, string templateId = null)
        {
            var result = await _dbContext2.TemplateAnalyticsDetailsList.FromSqlInterpolated($"EXEC usp_TemplateAnalytics_Ops @ActionId={(int)CrudEnum.GetTemplateAnalyticsDetails}, @ClientId={clientId},@SenderId={senderId},@TemplateIds={templateId},@StartDate={startDate},@EndDate={endDate}").ToListAsync();
            return result;
        }

    }
}
