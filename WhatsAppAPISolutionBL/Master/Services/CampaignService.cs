using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ITemplateService _templateService;
        private readonly HttpClient _httpClient;
        private readonly ICommunicationService _communicationService;

        public CampaignService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory,
            ICommunicationService communicationService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _communicationService = communicationService;
        }

        public async Task<List<UCampaign>> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @ClientId={1}, @CampaignId={2}, @FromDate='{3}', @ToDate='{4}', @SearchStr='{5}', @SortBy={6}, @PageNo={7}, @PageSize={8}, @SenderId={9}", (int)CrudEnum.List, ClientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize, SenderId);
            var response = await _dbContext2.Campaigns.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddCampaignAsync(CampaignDto campaign)
        {
            var campaignParamJson = JsonSerializer.Serialize(campaign.CampaignParameters);
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.Add}, @CampaignName={campaign.CampaignName}, @ClientId={campaign.ClientId}, @SenderId={campaign.SenderId}, @TemplateId={campaign.TemplateId}, @ScheduleDate={campaign.ScheduleDate}, @CampaignType={campaign.CampaignType}, @CampaignParamsJSON={campaignParamJson}, @CampaignContactsJSON={campaignContactJson}, @GroupIds={campaign.GroupIds}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> ActivateCampaignAsync(CampaignDto campaign)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @CampaignId={1}, @ClientId={2}, @ScheduleDate='{3}', @ActionBy={4}", (int)CrudEnum.ActivateCampaign, campaign.CampaignId, campaign.ClientId, campaign.ScheduleDate, campaign.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateCampaignAsync(CampaignDto campaign)
        {
            var campaignContactJson = JsonSerializer.Serialize(campaign.CampaignContacts);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Campaigns_Ops @ActionId={(int)CrudEnum.UpdateCampaign}, @CampaignId={campaign.CampaignId}, @GroupIds={campaign.GroupIds}, @CampaignContactsJSON={campaignContactJson}, @ActionBy={campaign.ActionBy}").ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            var query = string.Format(@"exec usp_Campaigns_Ops @ActionId={0}, @CampaignId={1}, @ClientId={2}", (int)CrudEnum.SettleCampaign, CampaignId, ClientId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SendCampaignMessagesAsync(SendCampaignDto campaign)
        {
            var campaignData = await _dbContext.Campaigns.Where(x => x.CampaignId == campaign.CampaignId).FirstOrDefaultAsync();
            if (campaignData == null)
                return new UResponse()
                {
                    Status = 0,
                    Message = "No campaign found with this Campaign Id"
                };
            if (campaignData.TemplateId <= 0)
                return new UResponse()
                {
                    Status = 0,
                    Message = "No template id found in this campaign please add template"
                };
            campaign.PhoneNumbers = campaign.PhoneNumbers.TrimPhoneNumbers(); 
            var tempPayload = new TemplateMessagePayloadDto()
            {
                ClientId = campaignData.ClientId,
                TemplateId = campaignData.TemplateId,
                PhoneNumbers = campaign.PhoneNumbers
            };
            tempPayload.Params = await _dbContext.CampaignParams.Where(x => x.CampaignId == campaign.CampaignId)
                .Select(x => new { x.ParamText, x.ParamType, x.Sequence }).OrderBy(x => x.Sequence)
                .Select(x => new ParamData
                {
                    ParamText = x.ParamText,
                    ParamType = x.ParamType
                }).ToListAsync();
            return await _communicationService.SendTemplateMessageAsync(tempPayload);

            //var campaignParams = await _dbContext.CampaignParams.Where(x => x.CampaignId == campaign.CampaignId).ToListAsync();
            //var headerParam = campaign != null ? campaignParams.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault()?.ParamText : "";
            //var bodyParams = campaign != null ? campaignParams.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList() : null;
            //var buttonParams = campaign != null ? campaignParams.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList() : null;
            //var bodyParameters = new Dictionary<int, string>();
            //var buttonParameters = new Dictionary<int, string>();
            //var sendMessage = new SendTemplateMessageDto()
            //{
            //    ClientId = templateDetails.ClientId.ToString(),
            //    SenderNameId = templateDetails.SenderId.ToString(),
            //    PhoneNumbers = campaign.PhoneNumbers,
            //    LanguageCode = templateDetails.Language,
            //    TemplateId = templateDetails.TemplateId,
            //    TemplateName = templateDetails.TemplateName
            //};
            //if (templateDetails.HeaderParamCount > 0 && !string.IsNullOrEmpty(headerParam))
            //{
            //    var headerComponents = new SendTemplateMessageDto.TemplateComponent()
            //    {
            //        ComponentType = TemplateParamEnum.Header.ToString()
            //    };
            //    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
            //    {
            //        Type = ((TemplateHeaderEnum)templateDetails.HeaderType).ToString(),
            //        Value = headerParam,
            //        Index = templateDetails.HeaderValue.Index
            //    });
            //    sendMessage.Components.Add(headerComponents);
            //}
            //else if (templateDetails.HeaderParamCount > 0 && string.IsNullOrEmpty(headerParam))
            //{
            //    return new UResponse()
            //    {
            //        Status = 0,
            //        Message = $"error - HParam is required."
            //    };
            //}
            //if (templateDetails.BodyParamCount > 0)
            //{
            //    var bodyComponents = new SendTemplateMessageDto.TemplateComponent()
            //    {
            //        ComponentType = TemplateParamEnum.Body.ToString()
            //    };
            //    for (int i = 0; i < bodyParams.Count; i++)
            //    {
            //        var param = bodyParams[i];
            //        var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

            //        // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
            //        bodyParameters.Add(i, paramValue);
            //    }
            //    for (int i = 0; i < templateDetails.BodyValues.Count(); i++)
            //    {
            //        // Check if the parameter for the given index is null or empty
            //        if (bodyParameters.ContainsKey(i))
            //        {
            //            var paramValue = bodyParameters[i];

            //            // If parameter is null or empty, throw an error
            //            if (string.IsNullOrEmpty(paramValue))
            //            {
            //                //throw new Exception($"Error: BParam{i + 1} is required when index is {i}.");
            //                return new UResponse()
            //                {
            //                    Status = 0,
            //                    Message = $"error - BParam{i + 1} is required when body parameter is greater than {i + 1}."
            //                };
            //            }

            //            // Add the parameter to the bodyComponents if it's valid
            //            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
            //            {
            //                Type = "text",
            //                Value = paramValue,
            //                Index = i
            //            });
            //        }
            //    }
            //    sendMessage.Components.Add(bodyComponents);
            //}
            //if (templateDetails.ButtonValues.Any())
            //{
            //    var buttonComponents = new SendTemplateMessageDto.TemplateComponent()
            //    {
            //        ComponentType = TemplateParamEnum.Button.ToString()
            //    };

            //    for (int i = 0; i < buttonParams.Count; i++)
            //    {
            //        var param = buttonParams[i];
            //        var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

            //        // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
            //        buttonParameters.Add(i, paramValue);
            //    }

            //    // Get the ordered list of ButtonValues
            //    var orderedButtonValues = templateDetails.ButtonValues.OrderBy(x => x.Sequence).ToList();

            //    for (int i = 0; i < orderedButtonValues.Count; i++)
            //    {
            //        // Check if the parameter for the given index is null or empty
            //        if (buttonParameters.ContainsKey(i))
            //        {
            //            var paramValue = buttonParameters[i];

            //            // If parameter is null or empty, throw an error
            //            if (string.IsNullOrEmpty(paramValue))
            //            {
            //                //throw new Exception($"Error: BtnParam{i + 1} is required when index is {i}.");
            //                return new UResponse()
            //                {
            //                    Status = 0,
            //                    Message = $"error - BParam{i + 1} is required when button parameter is greater than {i + 1}."
            //                };
            //            }

            //            // Add the parameter to the buttonComponents if it's valid
            //            buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
            //            {
            //                Type = ((ButtonTypeEnum)orderedButtonValues[i].Type).ToString(),
            //                Value = paramValue,
            //                Index = i
            //            });
            //        }
            //    }

            //    sendMessage.Components.Add(buttonComponents);
            //}
            //var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage), Encoding.UTF8, "application/json");
            //var response1 = await _httpClient.PostAsync($"/api/Template/SendBatchTemplateMessage", res);
            //var content = await response1.Content.ReadAsStringAsync();

            //var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            //if (result != null && result.success)
            //{
            //    var data = System.Text.Json.JsonSerializer.Serialize(result.result);
            //    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
            //    if (tempResult != null)
            //    {
            //        foreach (var item in tempResult)
            //        {

            //            if (item.success)
            //            {
            //                var message1 = new InsertMessageDto
            //                {
            //                    client_Id = campaignData.ClientId,
            //                    wam_Id = item.waId,
            //                    recipient_Id = item.phoneNumber,
            //                    status = MessageStatusEnum.SENT.ToString(),
            //                    module_Id = (int)ModuleEnum.Campaign,
            //                    template_Id = campaignData.TemplateId
            //                };
            //                message1.conversation.id = item.messageId;
            //                var response = await _communicationService.AddMessageSentLogAsync(message1);
            //            }
            //            else
            //            {
            //                var message1 = new InsertMessageDto
            //                {
            //                    client_Id = campaignData.ClientId,
            //                    wam_Id = item.waId,
            //                    recipient_Id = item.phoneNumber,
            //                    status = MessageStatusEnum.FAILED.ToString(),
            //                    module_Id = (int)ModuleEnum.Campaign,
            //                    template_Id = campaignData.TemplateId
            //                };
            //                message1.conversation.id = item.messageId;
            //                message1.error.error_Details = item.errors.ToString();
            //                var response = await _communicationService.AddMessageSentLogAsync(message1);
            //            }
            //        }
            //    }
            //}
            //else if (result != null && !result.success)
            //{
            //    return new UResponse()
            //    {
            //        Status = 0,
            //        Message = result.message
            //    };
            //}
            //return new UResponse()
            //{
            //    Status = 1,
            //    Message = "Campaign Sent Successfully"
            //};
        }
    }
}
