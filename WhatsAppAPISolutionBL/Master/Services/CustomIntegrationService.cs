using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Template;
using static WhatsAppAPISolutionDL.Dto.Message.WhatsAppMessageStatusUpdateDto;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CustomIntegrationService : ICustomIntegrationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ITemplateService _templateService;
        private readonly HttpClient _httpClient;
        private readonly IAPIMessageService _apiMessageService;
        private readonly IMessageService _messageService;
        private readonly ICommunicationService _communicationService;
        private readonly IMessageSentLogsService _messageSentLogsService;
        private readonly    IMediatorService _mediatorService;

        public CustomIntegrationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory,
            IAPIMessageService apiMessageService,
            IMessageService messageService,
            ICommunicationService communicationService,
            IMessageSentLogsService messageSentLogsService,
            IMediatorService mediatorService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _apiMessageService = apiMessageService;
            _messageService = messageService;
            _communicationService = communicationService;
            _messageSentLogsService = messageSentLogsService;
            _mediatorService = mediatorService;
        }

        public async Task<ApiResult> SendSmsAsync(SendSmsDto sendSms, int ClientId, int UserId)
        {
            sendSms.PhoneNumber = sendSms.PhoneNumber.TrimPhoneNumbers();
            var templateName = String.Empty;

            //If brand name is present than only append brand name
            if (!String.IsNullOrWhiteSpace(sendSms.BrandName))
            {
                sendSms.BrandName = sendSms.BrandName.Replace(" ", "_");
                templateName = String.Concat(sendSms.BrandName, "_", sendSms.TemplateName).ToLower();
            }
            else
                templateName = sendSms.TemplateName;

            string request = JsonConvert.SerializeObject(sendSms);
            var tempPayload = new TemplateMessagePayloadDto
            {
                ClientId = ClientId,
                UserId = UserId,
                PhoneNumbers = new List<string> { sendSms.PhoneNumber },
                IsApiMessage = true,
                Url = request,
                ParentId = 0,
                ModuleId = (int)ModuleEnum.API,
                UDF1 = sendSms.UDF1,
                UDF2 = sendSms.UDF2
            };

            //Get template id and sender id
            int senderId = 0;
            if (!String.IsNullOrWhiteSpace(templateName))
            {
                var template = await _dbContext.Templates.Where(x => x.ClientId == ClientId && x.TemplateName.ToLower() == templateName.ToLower()).FirstOrDefaultAsync();
                if (template == null)
                {
                    return new ApiResult
                    {
                        Message = $"No template found with given name {templateName}"
                    };
                }

                if (template != null)
                {
                    tempPayload.TemplateId = template.Id;
                    senderId = template.SenderId.HasValue ? template.SenderId.Value : 0;
                }
            }

            var paramList = new List<ParamData>();

            if (!String.IsNullOrWhiteSpace(sendSms.HParam))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Header, ParamValue = sendSms.HParam, Sequence = 0 });

            if (!String.IsNullOrWhiteSpace(sendSms.BParam1))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Body, ParamValue = sendSms.BParam1, Sequence = 0 });

            if (!String.IsNullOrWhiteSpace(sendSms.BParam2))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Body, ParamValue = sendSms.BParam2, Sequence = 1 });

            if (!String.IsNullOrWhiteSpace(sendSms.BParam3))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Body, ParamValue = sendSms.BParam3, Sequence = 2 });

            if (!String.IsNullOrWhiteSpace(sendSms.BParam4))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Body, ParamValue = sendSms.BParam4, Sequence = 3 });

            if (!String.IsNullOrWhiteSpace(sendSms.BtnParam1))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Button, ParamValue = sendSms.BtnParam1, Sequence = 0 });

            if (!String.IsNullOrWhiteSpace(sendSms.BtnParam2))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Button, ParamValue = sendSms.BtnParam2, Sequence = 1 });

            if (!String.IsNullOrWhiteSpace(sendSms.BtnParam3))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Button, ParamValue = sendSms.BtnParam3, Sequence = 2 });

            if (!String.IsNullOrWhiteSpace(sendSms.BtnParam4))
                paramList.Add(new ParamData { ParamType = (int)TemplateParamEnum.Button, ParamValue = sendSms.BtnParam4, Sequence = 3 });

            // Add each ParamData object to tempPayload.Params
            tempPayload.Params.AddRange(paramList);

            //entry in API message service
            //fetch APImessage primary key
            var message = new APIMessageDto
            {
                ClientId = tempPayload.ClientId,
                SenderNameId = senderId,
                TemplateId = tempPayload.TemplateId,
                PhoneNumber = sendSms.PhoneNumber,
                Status = 0,
                WaId = String.Empty,
                ActionBy = UserId,
                Url = tempPayload.Url,
                UDF1 = sendSms.UDF1,
                UDF2 = sendSms.UDF2,
            };

            var response = await _apiMessageService.AddAPIMessageAsync(message);
            if (response != null)
            
            {
                tempPayload.ParentId = response.Id;
            }

            var flowToken = $"{FlowIdentifier.ClientId}:{tempPayload.ClientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{tempPayload.ModuleId}|" + $"{FlowIdentifier.ParentId}:{tempPayload.ParentId}";
            tempPayload.FlowToken = flowToken;
            if (sendSms.IsForceSend == 0)
            {
                var template = await _templateService.GetTemplateDetailAsync(tempPayload.ClientId, tempPayload.TemplateId);
                if (template == null)
                    return new ApiResult { StatusCode = 0, Message = "Template not found or deleted" };
                DBResponse dbresponse = new DBResponse();
                dbresponse.ResponseType = 2;
                var manualTemplateDBResponse = new ManualTemplateDBResponse
                {
                    ActionId = template.Id,
                    ClientId = template.ClientId ?? 0,
                    SenderId = template.SenderId,
                    HeaderType = template.HeaderType?? 0,
                    MediaId = template.MediaId ?? 0,
                    BodyText = template.BodyText,
                    HeaderText = template.HeaderText,
                    FooterText = template.FooterText,
                    ActionType = (int)TemplateTypeEnum.Template,
                    PhoneNumber = message.PhoneNumber,
                    Buttons = template.Buttons.Select(x => new ManualTemplateDBResponse.Button
                    {
                        ButtonId = x.ButtonId.ToString(),
                        ButtonText = x.ButtonText,
                        ButtonValue = x.ButtonValue,
                        ButtonType = x.ButtonType ?? 0,
                        Sequence = x.Sequence ?? 0,
                        ActionId = x.ActionId ?? 0,
                        ActionType = x.ActionType ?? 0
                    }).ToList(),
                    Params = template.Parameters.Select( Parameter =>
                    {
                        var match = tempPayload.Params
                        .FirstOrDefault(pv => pv.ParamType == Parameter.ParamType && pv.Sequence == Parameter.Sequence);
                        return new ParamValue
                        {
                            Key = Parameter.ParamName,
                            Value = match?.ParamValue ?? ""
                        };

                    }).ToList()
                };

                // CONVERTING THE manualTemplateDBResponse AND PASSING IT INTO THE DbResponse Json 
                string json = JsonConvert.SerializeObject(manualTemplateDBResponse, Formatting.Indented);
                dbresponse.Json = json;
                 return await _mediatorService.ProcessDBResponse(template.ClientId ?? 0, template.SenderId, dbresponse);
                //return new ApiResult { StatusCode = 1, Message = result.Message, Result = result };
            }
            if(sendSms.IsForceSend == 1)
            {
                //Send in communication service 
                return await _communicationService.SendTemplateMessageAsync(tempPayload);
            }
            if(sendSms.IsForceSend ==2)
            {
                var chatresponse = await _dbContext2.UResponseWithConversationId.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.CheckActiveConversation},@PhoneNumber={sendSms.PhoneNumber}").ToListAsync();
               // _logger.LogInformation("Calling procedure usp_Conversations_Ops with phonenumber={ClientId},actionId={ActionId}",sendSms.PhoneNumber, CrudEnum.GetConversationLogs);

            }
             return await _communicationService.SendTemplateMessageAsync(tempPayload);
        }
    }
}
