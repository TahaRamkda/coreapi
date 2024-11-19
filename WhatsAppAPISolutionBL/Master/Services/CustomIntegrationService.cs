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
                if (templateDetails.BodyParamCount > 0)
                {
                    var bodyComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Body.ToString()
                    };
                    for (int i = 0; i <= templateDetails.BodyValues.Count(); i++)
                    {
                        if (i == 0 && !string.IsNullOrEmpty(sendSms.BParam1))
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.BParam1,
                                Index = i
                            });
                        }
                        if (i == 1 && !string.IsNullOrEmpty(sendSms.BParam2))
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.BParam2,
                                Index = i
                            });
                        }
                        if (i == 2 && !string.IsNullOrEmpty(sendSms.BParam3))
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.BParam3,
                                Index = i
                            });
                        }
                        if (i == 3 && !string.IsNullOrEmpty(sendSms.BParam4))
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.BParam4,
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
                    for (int i = 0; i <= templateDetails.ButtonValues.OrderBy(x => x.Sequence).Count(); i++)
                    {
                        if (i == 0 && !string.IsNullOrEmpty(sendSms.BtnParam1))
                        {
                            buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = ((ButtonTypeEnum)templateDetails.ButtonValues[i].Type).ToString(),
                                Value = sendSms.BtnParam1,
                                Index = i
                            });
                        }
                        if (i == 1 && !string.IsNullOrEmpty(sendSms.BtnParam2))
                        {
                            buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = ((ButtonTypeEnum)templateDetails.ButtonValues[i].Type).ToString(),
                                Value = sendSms.BtnParam2,
                                Index = i
                            });
                        }
                        if (i == 2 && !string.IsNullOrEmpty(sendSms.BtnParam3))
                        {
                            buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = ((ButtonTypeEnum)templateDetails.ButtonValues[i].Type).ToString(),
                                Value = sendSms.BtnParam3,
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
                        if (tempResult[0].success)
                        {

                        }
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
                                    status = "sent"
                                };
                                var response = await _messageService.UpdateMessageStatusAsync(message1);
                            }
                            else
                            {
                                var message1 = new WhatsAppMessageStatusUpdateDto()
                                {
                                    client_Id = ClientId.ToString(),
                                    wam_Id = item.waId,
                                    recipient_Id = item.phoneNumber,
                                    status = "failed"
                                };
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
                    Message = "Template name is not exist"
                };
            }
            return new UResponse()
            {
                Status = 1,
                Message = "Messages sent Successfully"
            };
        }
    }
}
