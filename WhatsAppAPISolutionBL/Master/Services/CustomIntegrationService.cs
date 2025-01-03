using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;

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

        public CustomIntegrationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory,
            IAPIMessageService apiMessageService,
            IMessageService messageService,
            ICommunicationService communicationService,
            IMessageSentLogsService messageSentLogsService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _apiMessageService = apiMessageService;
            _messageService = messageService;
            _communicationService = communicationService;
            _messageSentLogsService = messageSentLogsService;
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
                ModuleId = (int)ModuleEnum.API
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

            var paramList = new List<ParamData>
            {
                new ParamData { ParamText = sendSms.HParam, ParamType = (int)TemplateParamEnum.Header, Sequence = 1 },
                new ParamData { ParamText = sendSms.BParam1, ParamType = (int)TemplateParamEnum.Body, Sequence = 1  },
                new ParamData { ParamText = sendSms.BParam2, ParamType = (int)TemplateParamEnum.Body, Sequence = 2  },
                new ParamData { ParamText = sendSms.BParam3, ParamType = (int)TemplateParamEnum.Body, Sequence = 3  },
                new ParamData { ParamText = sendSms.BParam4, ParamType = (int)TemplateParamEnum.Body, Sequence = 4  },
                new ParamData { ParamText = sendSms.BtnParam1, ParamType = (int)TemplateParamEnum.Button, Sequence = 0  },
                new ParamData { ParamText = sendSms.BtnParam2, ParamType = (int)TemplateParamEnum.Button, Sequence = 1  },
                new ParamData { ParamText = sendSms.BtnParam3, ParamType = (int)TemplateParamEnum.Button, Sequence = 2  }
            };
             
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
                Url = tempPayload.Url
            };

            var response = await _apiMessageService.AddAPIMessageAsync(message);
            if (response != null)
            {
                tempPayload.ParentId = response.Id;
            }

            //Send in communication service 
            return await _communicationService.SendTemplateMessageAsync(tempPayload);
        }
    }
}
