using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly ITemplateService _templateService;
        private readonly IMessageSentLogsService _messageSentLogsService;
        private readonly IAPIMessageService _apiMessageService;

        public CommunicationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ITemplateService templateService,
            IMessageSentLogsService messageSentLogsService,
            IAPIMessageService apiMessageService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _templateService = templateService;
            _messageSentLogsService = messageSentLogsService;
            _apiMessageService = apiMessageService;
        }

        public async Task<UResponse> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage)
        {
            var templateDetails = await _templateService.GetTemplateDetailsAsync(templateMessage.ClientId, templateMessage.TemplateId, searchStr: templateMessage.TemplateName);
            if (templateDetails == null)
                return new UResponse()
                {
                    Status = 0,
                    Message = "Template not found or deleted"
                };

            var headerParam = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault()?.ParamText : "";
            var bodyParams = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList() : null;
            var buttonParams = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList() : null;

            var bodyParameters = new Dictionary<int, string>();
            var buttonParameters = new Dictionary<int, string>();

            var sendMessage = new SendTemplateMessageDto()
            {
                ClientId = templateDetails.ClientId.ToString(),
                SenderNameId = templateDetails.SenderId.ToString(),
                PhoneNumbers = templateMessage.PhoneNumbers,
                LanguageCode = templateDetails.Language,
                TemplateId = templateDetails.TemplateId,
                TemplateName = templateDetails.TemplateName
            };

            if (templateDetails.HeaderParamCount > 0 && !string.IsNullOrEmpty(headerParam))
            {
                var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                {
                    ComponentType = TemplateParamEnum.Header.ToString()
                };

                headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                {
                    Type = ((TemplateHeaderEnum)templateDetails.HeaderType).ToString(),
                    Value = headerParam,
                    Index = templateDetails.HeaderValue.Index
                });

                sendMessage.Components.Add(headerComponents);
            }
            else if (templateDetails.HeaderParamCount > 0 && string.IsNullOrEmpty(headerParam))
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

                for (int i = 0; i < bodyParams.Count; i++)
                {
                    var param = bodyParams[i];
                    var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

                    // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
                    bodyParameters.Add(i, paramValue);
                }

                for (int i = 0; i < templateDetails.BodyValues.Count(); i++)
                {
                    // Check if the parameter for the given index is null or empty
                    if (bodyParameters.ContainsKey(i))
                    {
                        var paramValue = bodyParameters[i];

                        // If parameter is null or empty, throw an error
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            //throw new Exception($"Error: BParam{i + 1} is required when index is {i}.");
                            return new UResponse()
                            {
                                Status = 0,
                                Message = $"error - BParam{i + 1} is required when body parameter is greater than {i}."
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

                for (int i = 0; i < buttonParams.Count; i++)
                {
                    var param = buttonParams[i];
                    var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

                    // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
                    buttonParameters.Add(i, paramValue);
                }

                // Get the ordered list of ButtonValues
                var orderedButtonValues = templateDetails.ButtonValues.OrderBy(x => x.Sequence).ToList();

                for (int i = 0; i < orderedButtonValues.Count; i++)
                {
                    if (orderedButtonValues[i].Type == (int)ButtonTypeEnum.URL && orderedButtonValues[i].IsDynamic)
                    {
                        // Check if the parameter for the given index is null or empty
                        if (buttonParameters.ContainsKey(i))
                        {
                            var paramValue = buttonParameters[i];

                            // If parameter is null or empty, throw an error
                            if (string.IsNullOrEmpty(paramValue))
                            {
                                //throw new Exception($"Error: BtnParam{i + 1} is required when index is {i}.");
                                return new UResponse()
                                {
                                    Status = 0,
                                    Message = $"error - BtnParam{i + 1} is required when button parameter is greater than {i}."
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
                }

                sendMessage.Components.Add(buttonComponents);
            }

            var request = Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage);
            var res = new StringContent(request, Encoding.UTF8, "application/json");
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
                        if (templateMessage.IsApiMessage)
                        {
                            var message = new APIMessageDto()
                            {
                                ClientId = templateMessage.ClientId,
                                SenderNameId = (int)templateDetails.SenderId,
                                TemplateId = (int)templateDetails.Id,
                                PhoneNumber = item.phoneNumber,
                                Status = item.status,
                                WaId = item.waId,
                                ActionBy = templateMessage.UserId,
                                Url = templateMessage.Url
                            };

                            await _apiMessageService.AddAPIMessageAsync(message);
                        }
                        if (item.success)
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = templateMessage.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.SENT,
                                module_Id = (int)ModuleEnum.Campaign,
                                template_Id = (int)templateDetails.Id,
                                parent_Id = templateMessage.ParentId
                            };

                            message1.conversation.id = item.messageId;

                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
                        }
                        else
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = templateMessage.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.FAILED,
                                module_Id = (int)ModuleEnum.Campaign,
                                template_Id = (int)templateDetails.Id,
                                parent_Id = templateMessage.ParentId
                            };

                            message1.conversation.id = item.messageId;
                            message1.error.error_Details = String.Join(',', item.errors);
                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
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

            return new UResponse()
            {
                Status = 1,
                Message = "Message Sent Successfully"
            };
        }

        public async Task<UResponse> SendMessageAsync(SendMessageRequestDto model)
        {
            model.Message = model.Message.Trim();
            model.PhoneNumbers = model.PhoneNumbers.TrimPhoneNumbers();
            string mediaId = "";
            if (model.Type == (int)MessageTypeEnum.IMAGE || model.Type == (int)MessageTypeEnum.DOCUMENT)
            {
                var media = await _dbContext.Medias.Where(x => x.Id == model.MediaId && x.RecordStatus != -1).FirstOrDefaultAsync();
                if (media != null)
                    mediaId = media.MediaId;
                else
                    return new UResponse
                    {
                        Status = 0,
                        Message = "No media found with this MediaId"
                    };
            }
            var request = new SendMessageToBridgeDto
            {
                ClientId = model.ClientId.ToString(),
                SenderNameId = model.SenderId.ToString(),
                Type = ((MessageTypeEnum)model.Type).ToString(),
                Message = model.Message,
                MediaId = mediaId,
                FileName = model.FileName,
                PhoneNumbers = model.PhoneNumbers,
            };

            var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync($"/api/Message/SendBatchMessage", res);
            var content = await response1.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    foreach (var item in tempResult)
                    {
                        if (item.success)
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = model.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.SENT,
                                module_Id = (int)ModuleEnum.Chat,
                                message_Type = model.Type
                            };

                            message1.conversation.id = item.messageId;

                            if (model.Type == 1)
                                message1.message_Text = model.Message;
                            else
                                message1.message_Text = model.MediaId.ToString();

                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
                        }
                        else
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = model.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.FAILED,
                                module_Id = (int)ModuleEnum.Chat,
                                message_Type = model.Type
                            };

                            message1.conversation.id = item.messageId;
                            message1.error.error_Details = String.Join(',', item.errors);

                            if (model.Type == 1)
                                message1.message_Text = model.Message;
                            else
                                message1.message_Text = model.MediaId.ToString();

                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
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
            return new UResponse()
            {
                Status = 1,
                Message = "Message Sent Successfully"
            };
        }

        public async Task<UResponse> SendInteractiveMessageAsync(TemplateMessagePayloadDto templateMessage)
        {
            var templateDetails = await _templateService.GetTemplateDetailsAsync(templateMessage.ClientId, templateMessage.TemplateId, searchStr: templateMessage.TemplateName);
            if (templateDetails == null)
                return new UResponse()
                {
                    Status = 0,
                    Message = "Template not found or deleted"
                };

            var headerParam = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault()?.ParamText : "";
            var bodyParams = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList() : null;
            var buttonParams = templateMessage.Params != null ? templateMessage.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList() : null;
            var bodyParameters = new Dictionary<int, string>();
            var buttonParameters = new Dictionary<int, string>();

            var sendMessage = new SendTemplateMessageDto()
            {
                ClientId = templateDetails.ClientId.ToString(),
                SenderNameId = templateDetails.SenderId.ToString(),
                PhoneNumbers = templateMessage.PhoneNumbers,
                LanguageCode = templateDetails.Language,
                TemplateId = templateDetails.TemplateId,
                TemplateName = templateDetails.TemplateName
            };

            if (templateDetails.HeaderParamCount > 0 && !string.IsNullOrEmpty(headerParam))
            {
                var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                {
                    ComponentType = TemplateParamEnum.Header.ToString()
                };

                headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                {
                    Type = ((TemplateHeaderEnum)templateDetails.HeaderType).ToString(),
                    Value = headerParam,
                    Index = templateDetails.HeaderValue.Index
                });

                sendMessage.Components.Add(headerComponents);
            }
            else if (templateDetails.HeaderParamCount > 0 && string.IsNullOrEmpty(headerParam))
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

                for (int i = 0; i < bodyParams.Count; i++)
                {
                    var param = bodyParams[i];
                    var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

                    // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
                    bodyParameters.Add(i, paramValue);
                }

                for (int i = 0; i < templateDetails.BodyValues.Count(); i++)
                {
                    // Check if the parameter for the given index is null or empty
                    if (bodyParameters.ContainsKey(i))
                    {
                        var paramValue = bodyParameters[i];

                        // If parameter is null or empty, throw an error
                        if (string.IsNullOrEmpty(paramValue))
                        {
                            //throw new Exception($"Error: BParam{i + 1} is required when index is {i}.");
                            return new UResponse()
                            {
                                Status = 0,
                                Message = $"error - BParam{i + 1} is required when body parameter is greater than {i}."
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

                for (int i = 0; i < buttonParams.Count; i++)
                {
                    var param = buttonParams[i];
                    var paramValue = param.ParamText; // Use ParamDefaultValue or another field to get the parameter's value

                    // Optionally, you can use ParamName, ParamText, or ParamDefaultValue to get the value
                    buttonParameters.Add(i, paramValue);
                }

                // Get the ordered list of ButtonValues
                var orderedButtonValues = templateDetails.ButtonValues.OrderBy(x => x.Sequence).ToList();

                for (int i = 0; i < orderedButtonValues.Count; i++)
                {
                    if (orderedButtonValues[i].Type == (int)ButtonTypeEnum.URL && orderedButtonValues[i].IsDynamic)
                    {
                        // Check if the parameter for the given index is null or empty
                        if (buttonParameters.ContainsKey(i))
                        {
                            var paramValue = buttonParameters[i];

                            // If parameter is null or empty, throw an error
                            if (string.IsNullOrEmpty(paramValue))
                            {
                                //throw new Exception($"Error: BtnParam{i + 1} is required when index is {i}.");
                                return new UResponse()
                                {
                                    Status = 0,
                                    Message = $"error - BtnParam{i + 1} is required when button parameter is greater than {i}."
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
                        if (templateMessage.IsApiMessage)
                        {
                            var message = new APIMessageDto()
                            {
                                ClientId = templateMessage.ClientId,
                                SenderNameId = (int)templateDetails.SenderId,
                                TemplateId = (int)templateDetails.Id,
                                PhoneNumber = item.phoneNumber,
                                Status = item.status,
                                WaId = item.waId,
                                ActionBy = templateMessage.UserId,
                                Url = templateMessage.Url
                            };

                            await _apiMessageService.AddAPIMessageAsync(message);
                        }
                        if (item.success)
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = templateMessage.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.SENT,
                                module_Id = (int)ModuleEnum.Campaign,
                                template_Id = (int)templateDetails.Id
                            };

                            message1.conversation.id = item.messageId;
                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
                        }
                        else
                        {
                            var message1 = new InsertMessageDto
                            {
                                client_Id = templateMessage.ClientId,
                                wam_Id = item.waId,
                                recipient_Id = item.phoneNumber,
                                status = MessageStatusEnum.FAILED,
                                module_Id = (int)ModuleEnum.Campaign,
                                template_Id = (int)templateDetails.Id
                            };

                            message1.conversation.id = item.messageId;
                            message1.error.error_Details = item.errors.ToString();
                            await _messageSentLogsService.AddMessageSentLogAsync(message1);
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
            return new UResponse()
            {
                Status = 1,
                Message = "Message Sent Successfully"
            };
        }
    }
}
