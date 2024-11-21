using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using static WhatsAppAPISolutionDL.Dto.WhatsAppMessageStatusUpdateDto;

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

        public CustomIntegrationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory,
            IAPIMessageService apiMessageService,
            IMessageService messageService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient("bridge_api");
            _apiMessageService = apiMessageService;
            _messageService = messageService;
        }

        public async Task<UResponse> SendSmsAsync(SendSmsDto sendSms, int ClientId, int UserId)
        {
            sendSms.BrandName = sendSms.BrandName.Replace(" ", "_");
            sendSms.PhoneNumber = sendSms.PhoneNumber.Replace("+", "");
            var templateName = string.Concat(sendSms.BrandName, "_", sendSms.TemplateName).ToLower();
            var templateDetails = await _templateService.GetTemplateDetailsAsync(client_Id: ClientId, searchStr: templateName);
            if (templateDetails != null)
            {
                var sendMessage = new SendTemplateMessageDto()
                {
                    ClientId = templateDetails.ClientId.ToString(),
                    SenderNameId = templateDetails.SenderId.ToString(),
                    PhoneNumbers = new List<string> { sendSms.PhoneNumber },
                    LanguageCode = templateDetails.Language,
                    TemplateId = templateDetails.TemplateId,
                    TemplateName = templateDetails.TemplateName
                };
                if (templateDetails.HeaderParamCount > 0 && !string.IsNullOrEmpty(sendSms.HParam))
                {
                    var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Header.ToString()
                    };
                    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = ((TemplateHeaderEnum)templateDetails.HeaderType).ToString(),
                        Value = sendSms.HParam,
                        Index = templateDetails.HeaderValue.Index
                    });
                    sendMessage.Components.Add(headerComponents);
                }
                else if (templateDetails.HeaderParamCount > 0 && string.IsNullOrEmpty(sendSms.HParam))
                {
                    return new UResponse()
                    {
                        Status = 0,
                        Message = $"error - HParam is required."
                    };
                }
                if (templateDetails.BodyParamCount > 0)
                {
                    var bodyComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Body.ToString()
                    };
                    var parameters = new Dictionary<int, string>
                    {
                        { 0, sendSms.BParam1 },
                        { 1, sendSms.BParam2 },
                        { 2, sendSms.BParam3 },
                        { 3, sendSms.BParam4 }
                    };
                    for (int i = 0; i < templateDetails.BodyValues.Count(); i++)
                    {
                        // Check if the parameter for the given index is null or empty
                        if (parameters.ContainsKey(i))
                        {
                            var paramValue = parameters[i];

                            // If parameter is null or empty, throw an error
                            if (string.IsNullOrEmpty(paramValue))
                            {
                                //throw new Exception($"Error: BParam{i + 1} is required when index is {i}.");
                                return new UResponse()
                                {
                                    Status = 0,
                                    Message = $"error - BParam{i + 1} is required when body parameter is greater than {i + 1}."
                                };
                            }

                            // Add the parameter to the bodyComponents if it's valid
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = paramValue,
                                Index = i
                            });
                        }
                    }
                    sendMessage.Components.Add(bodyComponents);
                }
                if (templateDetails.ButtonValues.Any())
                {
                    var buttonComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Button.ToString()
                    };

                    var buttonParams = new Dictionary<int, string>
                    {
                        { 0, sendSms.BtnParam1 },
                        { 1, sendSms.BtnParam2 },
                        { 2, sendSms.BtnParam3 }
                    };

                    // Get the ordered list of ButtonValues
                    var orderedButtonValues = templateDetails.ButtonValues.OrderBy(x => x.Sequence).ToList();

                    for (int i = 0; i < orderedButtonValues.Count; i++)
                    {
                        // Check if the parameter for the given index is null or empty
                        if (buttonParams.ContainsKey(i))
                        {
                            var paramValue = buttonParams[i];

                            // If parameter is null or empty, throw an error
                            if (string.IsNullOrEmpty(paramValue))
                            {
                                //throw new Exception($"Error: BtnParam{i + 1} is required when index is {i}.");
                                return new UResponse()
                                {
                                    Status = 0,
                                    Message = $"error - BParam{i + 1} is required when button parameter is greater than {i + 1}."
                                };
                            }

                            // Add the parameter to the buttonComponents if it's valid
                            buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = ((ButtonTypeEnum)orderedButtonValues[i].Type).ToString(),
                                Value = paramValue,
                                Index = i
                            });
                        }
                    }

                    sendMessage.Components.Add(buttonComponents);
                }
                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/SendBatchTemplateMessage", res);
                var content = await response1.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                    if (tempResult != null)
                    {
                        foreach (var item in tempResult)
                        {
                            var message = new APIMessageDto()
                            {
                                ClientId = ClientId,
                                SenderNameId = (int)templateDetails.SenderId,
                                TemplateId = (int)templateDetails.TemplatesId,
                                PhoneNumber = item.phoneNumber,
                                Status = item.status,
                                WaId = item.waId,
                                ActionBy = UserId
                            };
                            await _apiMessageService.AddAPIMessageAsync(message);

                            if (item.success)
                            {
                                var message1 = new WhatsAppMessageStatusUpdateDto()
                                {
                                    client_Id = ClientId.ToString(),
                                    wam_Id = item.waId,
                                    recipient_Id = item.phoneNumber,
                                    status = MessageStatusEnum.SENT.ToString()
                                };
                                message1.conversation.id = item.messageId;
                                var response = await _messageService.UpdateMessageStatusAsync(message1);
                            }
                            else
                            {
                                var message1 = new WhatsAppMessageStatusUpdateDto()
                                {
                                    client_Id = ClientId.ToString(),
                                    wam_Id = item.waId,
                                    recipient_Id = item.phoneNumber,
                                    status = MessageStatusEnum.FAILED.ToString()
                                };
                                message1.conversation.id = item.messageId;
                                message1.error.error_Details = item.errors.ToString();
                                var response = await _messageService.UpdateMessageStatusAsync(message1);
                            }
                        }
                    }
                }
                else if (result != null && !result.success)
                {
                    return new UResponse()
                    {
                        Status = 0,
                        Message = result.message
                    };
                }
            }
            else
            {
                return new UResponse()
                {
                    Status = 0,
                    Message = "error - Template name is not exist"
                };
            }
            return new UResponse()
            {
                Status = 1,
                Message = "success - Messages sent Successfully"
            };
        }
    }
}
