using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
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
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;

        public CommunicationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ITemplateService templateService,
            IMessageSentLogsService messageSentLogsService,
            IAPIMessageService apiMessageService,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _templateService = templateService;
            _messageSentLogsService = messageSentLogsService;
            _apiMessageService = apiMessageService;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
        }

        public async Task<ApiResult> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage)
        {
            var templateDetails = await _templateService.GetTemplateDetailsAsync(templateMessage.ClientId, templateMessage.TemplateId);
            if (templateDetails == null)
                return new ApiResult
                {
                    StatusCode = 0,
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

                var headerType = (TemplateHeaderEnum)templateDetails.HeaderType;
                if (headerType == TemplateHeaderEnum.TEXT)
                {
                    if (templateDetails.HeaderParamCount > 0 && string.IsNullOrEmpty(headerParam))
                    {
                        return new ApiResult
                        {
                            StatusCode = 0,
                            Message = $"error - HParam is required."
                        };
                    }

                    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = headerType.ToString(),
                        Value = headerParam,
                        Index = templateDetails.HeaderValue.Index
                    });
                }
                else if (headerType == TemplateHeaderEnum.IMAGE
                    || headerType == TemplateHeaderEnum.DOCUMENT
                    || headerType == TemplateHeaderEnum.VIDEO)
                {
                    var media = _dbContext.Medias.Find(templateDetails.MediaId);
                    if (media != null)
                    {
                        var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                        headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                        {
                            Type = headerType.ToString(),
                            Value = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath,
                            Index = templateDetails.HeaderValue.Index
                        });
                    }
                }

                sendMessage.Components.Add(headerComponents);
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
                            return new ApiResult
                            {
                                StatusCode = 0,
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
                                return new ApiResult
                                {
                                    StatusCode = 0,
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

            var request = JsonConvert.SerializeObject(sendMessage);
            var res = new StringContent(request, Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync($"/api/Template/SendBatchTemplateMessage", res);
            var content = await response1.Content.ReadAsStringAsync();

            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);

            List<CustomIntegrationResult> models = new List<CustomIntegrationResult>();
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    foreach (var item in tempResult)
                    {
                        var model = new CustomIntegrationResult
                        {
                            Sent = item.success,
                            PhoneNumber = item.phoneNumber,
                            WaId = item.waId
                        };

                        var message = new InsertMessageDto
                        {
                            ClientId = templateMessage.ClientId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = templateMessage.ModuleId,
                            TemplateId = (int)templateDetails.Id,
                            ParentId = templateMessage.ParentId
                        };

                        if (item.errors != null && item.errors.Any())
                        {
                            string errors = String.Join(',', item.errors);
                            message.Error = new InsertMessageDto.ErrorDto
                            {
                                ErrorDetails = errors
                            };

                            model.Errors = errors;
                        }

                        await _messageSentLogsService.AddMessageSentLogAsync(message);


                        //Add result to custom integration result models
                        models.Add(model);
                    }
                }
            }

            //If custom integration models exist
            if (models.Any())
            {
                return new ApiResult
                {
                    Success = true,
                    StatusCode = 200,
                    Result = models,
                    Message = "Processed"
                };
            }

            return new ApiResult
            {
                StatusCode = 0,
                Message = result.message
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
                {
                    var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                    mediaId = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath;
                }
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

            var requestStr = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/Message/SendBatchMessage", requestStr);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    int messageTypeId = 0;
                    if (model.Type > 0) //Based on message type, set message type id in DB
                    {
                        switch ((MessageTypeEnum)model.Type)
                        {
                            case MessageTypeEnum.TEXT:
                                messageTypeId = 1; break;
                            case MessageTypeEnum.IMAGE:
                            case MessageTypeEnum.VIDEO:
                            case MessageTypeEnum.DOCUMENT:
                                messageTypeId = 2; break;
                            case MessageTypeEnum.LOCATION:
                                messageTypeId = 3; break;
                            default: break;
                        }
                    }

                    foreach (var item in tempResult)
                    {
                        var message = new InsertMessageDto
                        {
                            ClientId = model.ClientId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId,
                            TemplateId = model.ActionId,
                            ParentId = model.ParentId,
                            MessageType = messageTypeId,
                            MessageText = model.Message,
                            MediaId = model.MediaId.HasValue ? model.MediaId.Value : 0
                        };

                        if (item.errors != null && item.errors.Any())
                        {
                            message.Error = new InsertMessageDto.ErrorDto
                            {
                                ErrorDetails = String.Join(',', item.errors)
                            };
                        }

                        await _messageSentLogsService.AddMessageSentLogAsync(message);
                    }
                }
            }
            else if (result != null && !result.success)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = result.message
                };
            }
            return new UResponse
            {
                Status = 1,
                Message = "Message Sent Successfully"
            };
        }

        public async Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, string phoneNumber)
        {
            var templateDetails = await _templateService.GetTemplateDetailsAsync(clientId, model.ActionId == null ? 0 : model.ActionId.Value);
            if (templateDetails == null)
                return new UResponse
                {
                    Status = 0,
                    Message = "Template not found or deleted"
                };

            var headerType = (TemplateHeaderEnum)templateDetails.HeaderType;

            //In interactive button is required, if not available send normal message
            if (templateDetails.ButtonValues == null || !templateDetails.ButtonValues.Any()
                && !String.IsNullOrWhiteSpace(templateDetails.BodyText))
            {
                return await SendMessageAsync(new SendMessageRequestDto
                {
                    ClientId = clientId,
                    SenderId = templateDetails.SenderId,
                    MediaId = templateDetails.MediaId,
                    ModuleId = model.ModuleId ?? 0,
                    ParentId = model.ParentId ?? 0,
                    ActionId = model.ActionId ?? 0,
                    Type = (int)headerType,
                    Message = !String.IsNullOrWhiteSpace(templateDetails.HeaderText) ? String.Concat(templateDetails.HeaderText, "\n \n", templateDetails.BodyText) : templateDetails.BodyText,
                    FileName = templateDetails.FileName,
                    PhoneNumbers = new List<string> { phoneNumber }.TrimPhoneNumbers()
                });
            }

            var sendMessage = new SendInteractiveMessageRequestDto
            {
                ClientId = templateDetails.ClientId.ToString(),
                SenderNameId = templateDetails.SenderId.ToString(),
                PhoneNumbers = new List<string> { phoneNumber }.TrimPhoneNumbers()
            };

            if (headerType == TemplateHeaderEnum.TEXT)
            {
                sendMessage.Header = new SendInteractiveMessageRequestDto.HeaderDto
                {
                    Format = headerType.ToString(),
                    Value = templateDetails.HeaderText
                };
            }
            else if (headerType == TemplateHeaderEnum.IMAGE
                || headerType == TemplateHeaderEnum.DOCUMENT
                || headerType == TemplateHeaderEnum.VIDEO)
            {
                var media = _dbContext.Medias.Find(templateDetails.MediaId);
                if (media != null)
                {
                    var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                    sendMessage.Header = new SendInteractiveMessageRequestDto.HeaderDto
                    {
                        Format = headerType.ToString(),
                        Value = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath
                    };
                }
            }

            if (!String.IsNullOrWhiteSpace(templateDetails.BodyText))
            {
                sendMessage.Body = new SendInteractiveMessageRequestDto.BodyDto
                {
                    Text = templateDetails.BodyText.Trim()
                };
            }

            if (!String.IsNullOrWhiteSpace(templateDetails.FooterText))
            {
                sendMessage.Footer = new SendInteractiveMessageRequestDto.FooterDto
                {
                    Text = templateDetails.FooterText.Trim()
                };
            }

            if (templateDetails.ButtonValues.Any())
            {
                sendMessage.Buttons = new List<SendInteractiveMessageRequestDto.ButtonDto>();
                for (int i = 0; i < templateDetails.ButtonValues.Count; i++)
                {
                    var button = templateDetails.ButtonValues[i];
                    var buttonType = ((ButtonTypeEnum)button.Type);

                    if (buttonType != ButtonTypeEnum.QUICK_REPLY
                        && buttonType != ButtonTypeEnum.URL
                        && buttonType != ButtonTypeEnum.PHONE_NUMBER)
                        continue;

                    //In interactive, phone number is not 
                    if (buttonType == ButtonTypeEnum.PHONE_NUMBER)
                    {
                        buttonType = ButtonTypeEnum.URL;
                        button.Url = String.Concat("tel:", button.Text);
                    }

                    sendMessage.Buttons.Add(new SendInteractiveMessageRequestDto.ButtonDto
                    {
                        Id = !String.IsNullOrWhiteSpace(button.ButtonId) ? button.ButtonId : $"button_{i}",
                        Text = button.Text,
                        Type = buttonType.ToString(),
                        Url = button.Url
                    });
                }
            }

            var request = JsonConvert.SerializeObject(sendMessage);
            var requestStr = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/api/Message/SendInteractiveMessage", requestStr);
            var content = await response.Content.ReadAsStringAsync();

            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    foreach (var item in tempResult)
                    {
                        var message = new InsertMessageDto
                        {
                            ClientId = clientId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId.HasValue ? model.ModuleId.Value : 0,
                            TemplateId = model.ActionId.HasValue ? model.ActionId.Value : 0,
                            ParentId = model.ParentId.HasValue ? model.ParentId.Value : 0,
                            MediaId = templateDetails.MediaId.HasValue ? templateDetails.MediaId.Value : 0
                        };

                        if (item.errors != null && item.errors.Any())
                        {
                            message.Error = new InsertMessageDto.ErrorDto
                            {
                                ErrorDetails = String.Join(',', item.errors)
                            };
                        }

                        await _messageSentLogsService.AddMessageSentLogAsync(message);
                    }
                }
            }
            else if (result != null && !result.success)
            {
                return new UResponse
                {
                    Status = 0,
                    Message = result.message
                };
            }

            return new UResponse
            {
                Status = 1,
                Message = "Message Sent Successfully"
            };
        }
    }
}
