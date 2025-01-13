using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly ITemplateService _templateService;
        private readonly IInteractiveTemplateService _interactiveTemplateService;
        private readonly IMessageSentLogsService _messageSentLogsService;
        private readonly IMediaService _mediaService;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;

        public CommunicationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ITemplateService templateService,
            IMessageSentLogsService messageSentLogsService,
            IInteractiveTemplateService interactiveTemplateService,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IMediaService mediaService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _templateService = templateService;
            _interactiveTemplateService = interactiveTemplateService;
            _messageSentLogsService = messageSentLogsService;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _mediaService = mediaService;
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

            var headerType = (TemplateHeaderEnum)templateDetails.HeaderType;
            if (headerType == TemplateHeaderEnum.TEXT && templateDetails.HeaderParamCount > 0)
            {
                if (String.IsNullOrEmpty(headerParam))
                {
                    return new ApiResult
                    {
                        StatusCode = 0,
                        Message = $"error - HParam is required."
                    };
                }

                var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                {
                    ComponentType = TemplateParamEnum.Header.ToString()
                };

                headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                {
                    Type = headerType.ToString(),
                    Value = headerParam,
                    Index = templateDetails.HeaderValue != null ? templateDetails.HeaderValue.Index : 0
                });

                sendMessage.Components.Add(headerComponents);
            }
            else if (headerType == TemplateHeaderEnum.IMAGE
                    || headerType == TemplateHeaderEnum.DOCUMENT
                    || headerType == TemplateHeaderEnum.VIDEO)
            {
                var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                {
                    ComponentType = TemplateParamEnum.Header.ToString()
                };

                var media = _dbContext.Medias.Find(templateMessage.MediaId > 0 ? templateMessage.MediaId : templateDetails.MediaId); //If in campaign media id is present take reference from there, else default media
                if (media != null)
                {
                    var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = headerType.ToString(),
                        Value = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath,
                        Index = templateDetails.HeaderValue != null ? templateDetails.HeaderValue.Index : 0
                    });
                }
                else
                {
                    return new ApiResult
                    {
                        Message = $"error - Cannot find template media."
                    };
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
                    buttonParameters.Add(param.Sequence ?? 0, paramValue);
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
                            ClientId = templateDetails.ClientId ?? 0,
                            SenderId = templateDetails.SenderId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = templateMessage.ModuleId,
                            ParentId = templateMessage.ParentId,
                            MessageType = (int)MainMessageTypeEnum.TEMPLATE,
                            MessageReferenceId = (int)templateDetails.Id
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
            model.MessageContent = (model.MessageContent ?? "").Trim();
            model.PhoneNumbers = model.PhoneNumbers.TrimPhoneNumbers();
            string mediaId = "";

            if (model.Type == (int)MessageTypeEnum.IMAGE
                || model.Type == (int)MessageTypeEnum.DOCUMENT
                || model.Type == (int)MessageTypeEnum.VIDEO)
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
                Message = model.MessageContent,
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
                    foreach (var item in tempResult)
                    {
                        var message = new InsertMessageDto
                        {
                            ClientId = model.ClientId,
                            SenderId = model.SenderId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId,
                            ParentId = model.ParentId,
                            MessageReferenceId = model.MessageReferenceId,
                            MessageType = model.MessageTypeId,
                            MessageContent = model.MessageContent,
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

        public async Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, int senderId, string phoneNumber, int mediaId = 0, List<ParamValue> values = null)
        {
            var interactiveTemplate = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, model.ActionId ?? 0);
            if (interactiveTemplate == null)
                return new UResponse
                {
                    Status = 0,
                    Message = "Interactive template not found"
                };

            Media media = null;
            var headerType = (TemplateHeaderEnum)interactiveTemplate.HeaderType;

            //Try to fetch the media
            if (headerType == TemplateHeaderEnum.IMAGE
                || headerType == TemplateHeaderEnum.VIDEO
                || headerType == TemplateHeaderEnum.DOCUMENT) //Try to get the media
                media = await _dbContext.Medias.FindAsync(mediaId > 0 ? mediaId : interactiveTemplate.MediaId);

            string headerText = interactiveTemplate.HeaderText ?? "";
            string bodyText = interactiveTemplate.BodyText ?? "";
            string footerText = interactiveTemplate.FooterText ?? "";

            //Replace all the dynamic values with the correct one
            if (values != null && values.Any())
            {
                foreach (var value in values)
                {
                    headerText = headerText.Replace(value.Key, value.Value);
                    bodyText = bodyText.Replace(value.Key, value.Value);
                    footerText = footerText.Replace(value.Key, value.Value);

                    if (interactiveTemplate.Buttons != null && interactiveTemplate.Buttons.Any())
                    {
                        foreach (var button in interactiveTemplate.Buttons)
                        {
                            button.ButtonValue = button.ButtonValue.Replace(value.Key, value.Value);
                        }
                    }
                }
            }

            int type = 0;
            switch (headerType)
            {
                case TemplateHeaderEnum.TEXT:
                    type = (int)MessageTypeEnum.TEXT;
                    break;
                case TemplateHeaderEnum.IMAGE:
                    type = (int)MessageTypeEnum.IMAGE;
                    break;
                case TemplateHeaderEnum.VIDEO:
                    type = (int)MessageTypeEnum.VIDEO;
                    break;
                case TemplateHeaderEnum.DOCUMENT:
                    type = (int)MessageTypeEnum.DOCUMENT;
                    break;
                case TemplateHeaderEnum.LOCATION:
                    type = (int)MessageTypeEnum.LOCATION;
                    break;
                default:
                    break;
            }

            StringBuilder messageContent = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(headerText))
            {
                messageContent.Append(headerText);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(bodyText))
            {
                messageContent.Append(bodyText);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(footerText))
            {
                messageContent.Append(footerText);
            }

            //In interactive button is required, if not available send normal message
            if (interactiveTemplate.Buttons == null || !interactiveTemplate.Buttons.Any() && !String.IsNullOrWhiteSpace(interactiveTemplate.BodyText))
            {
                return await SendMessageAsync(new SendMessageRequestDto
                {
                    ClientId = interactiveTemplate.ClientId ?? 0,
                    SenderId = interactiveTemplate.SenderId ?? 0,
                    MediaId = media != null ? media.Id : 0,
                    ModuleId = model.ModuleId ?? 0,
                    ParentId = model.ParentId ?? 0,
                    ActionId = model.ActionId ?? 0,
                    Type = type,
                    MessageTypeId = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                    MessageReferenceId = interactiveTemplate.Id,
                    MessageContent = messageContent.ToString(),
                    FileName = media != null ? media.FileName : String.Empty,
                    PhoneNumbers = new List<string> { phoneNumber }.TrimPhoneNumbers()
                });
            }

            var sendMessage = new SendInteractiveMessageRequestDto
            {
                ClientId = Convert.ToString(interactiveTemplate.ClientId),
                SenderNameId = Convert.ToString(interactiveTemplate.SenderId),
                PhoneNumbers = new List<string> { phoneNumber }.TrimPhoneNumbers()
            };

            if (headerType == TemplateHeaderEnum.TEXT)
            {
                sendMessage.Header = new SendInteractiveMessageRequestDto.HeaderDto
                {
                    Format = headerType.ToString(),
                    Value = headerText
                };
            }
            else if (headerType == TemplateHeaderEnum.IMAGE
                || headerType == TemplateHeaderEnum.DOCUMENT
                || headerType == TemplateHeaderEnum.VIDEO)
            {
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

            if (!String.IsNullOrWhiteSpace(bodyText))
            {
                sendMessage.Body = new SendInteractiveMessageRequestDto.BodyDto
                {
                    Text = bodyText
                };
            }

            if (!String.IsNullOrWhiteSpace(footerText))
            {
                sendMessage.Footer = new SendInteractiveMessageRequestDto.FooterDto
                {
                    Text = footerText
                };
            }

            if (interactiveTemplate.Buttons != null && interactiveTemplate.Buttons.Any())
            {
                sendMessage.Buttons = new List<SendInteractiveMessageRequestDto.ButtonDto>();
                for (int i = 0; i < interactiveTemplate.Buttons.Count; i++)
                {
                    var button = interactiveTemplate.Buttons[i];
                    var buttonType = ((ButtonTypeEnum)button.ButtonType);

                    if (buttonType != ButtonTypeEnum.QUICK_REPLY
                        && buttonType != ButtonTypeEnum.URL
                        && buttonType != ButtonTypeEnum.PHONE_NUMBER)
                        continue;

                    //In interactive, phone number is not available, so we are making as URL
                    if (buttonType == ButtonTypeEnum.PHONE_NUMBER)
                    {
                        buttonType = ButtonTypeEnum.URL;
                        button.ButtonValue = String.Concat("tel:", button.ButtonValue ?? "").Replace("-", "").Trim(); //Replace +965-99310864
                    }

                    sendMessage.Buttons.Add(new SendInteractiveMessageRequestDto.ButtonDto
                    {
                        Id = Convert.ToString(button.ButtonId),
                        Text = button.ButtonText,
                        Type = buttonType.ToString(),
                        Url = button.ButtonValue
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
                        string buttonJson = String.Empty;

                        if (interactiveTemplate.Buttons != null && interactiveTemplate.Buttons.Any())
                        {
                            buttonJson = JsonConvert.SerializeObject(interactiveTemplate.Buttons.Select(x =>
                            new
                            {
                                ButtonId = x.ButtonId,
                                ButtonText = x.ButtonText,
                                ButtonValue = x.ButtonValue,
                                ButtonType = x.ButtonType,
                                Sequence = x.Sequence
                            }).ToList());
                        }

                        var message = new InsertMessageDto
                        {
                            ClientId = interactiveTemplate.ClientId ?? 0,
                            SenderId = interactiveTemplate.SenderId ?? 0,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId ?? 0,
                            ParentId = model.ParentId ?? 0,
                            MessageType = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                            MessageReferenceId = interactiveTemplate.Id,
                            MessageContent = messageContent.ToString(),
                            ButtonJson = buttonJson,
                            MediaId = media != null ? media.Id : 0
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

        public async Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model)
        {
            model.Message = (model.Message ?? "").Trim();

            var conversation = await _dbContext.Conversations.FindAsync(model.ConversationId);
            if (conversation == null || String.IsNullOrWhiteSpace(conversation.PhoneNumber))
            {
                return new ApiResult
                {
                    Message = "No conversation found"
                };
            }

            string mediaIdStr = String.Empty;
            string fileName = String.Empty;
            MessageTypeEnum messageType = MessageTypeEnum.TEXT;
            if (model.MediaId > 0)
            {
                var media = await _dbContext.Medias.FindAsync(model.MediaId);
                if (media == null || String.IsNullOrWhiteSpace(media.MediaId))
                {
                    return new ApiResult
                    {
                        Message = "Cannot upload media, Please try again"
                    };
                }

                mediaIdStr = media.MediaId;
                fileName = media.FileName;
                messageType = await _mediaService.GetMessageTypeFromMedia(media.Id);
            }

            var request = new SendMessageToBridgeDto
            {
                ClientId = model.ClientId.ToString(),
                SenderNameId = model.SenderId.ToString(),
                Type = messageType.ToString(),
                Message = model.Message,
                MediaId = mediaIdStr,
                FileName = fileName,
                PhoneNumbers = new List<string> { conversation.PhoneNumber },
            };

            var requestStr = JsonConvert.SerializeObject(request);
            var response = await _httpClient.PostAsync($"/api/Message/SendBatchMessage", new StringContent(requestStr, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    int messageTypeId = 0;

                    switch (messageType)
                    {
                        case MessageTypeEnum.TEXT:
                            messageTypeId = (int)MainMessageTypeEnum.TEXT;
                            break;
                        case MessageTypeEnum.IMAGE:
                        case MessageTypeEnum.VIDEO:
                        case MessageTypeEnum.DOCUMENT:
                            messageTypeId = (int)MainMessageTypeEnum.MEDIA;
                            break;
                        case MessageTypeEnum.LOCATION:
                            messageTypeId = (int)MainMessageTypeEnum.LOCATION;
                            break;
                        default: break;
                    }

                    foreach (var item in tempResult)
                    {
                        var message = new InsertMessageDto
                        {
                            ClientId = model.ClientId,
                            SenderId = model.SenderId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = (int)ModuleEnum.Chat,
                            ParentId = model.ConversationId,
                            MessageType = messageTypeId,
                            MessageReferenceId = 0,
                            MessageContent = model.Message,
                            MediaId = model.MediaId
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
                return new ApiResult
                {
                    StatusCode = 0,
                    Message = result.message
                };
            }

            return new ApiResult
            {
                StatusCode = 1,
                Message = "Message Sent Successfully"
            };
        }

        public async Task<ApiResult> SendAgentInteractiveMessageAsync(SendAgentInteractiveMessageRequestDto model)
        {
            var conversation = await _dbContext.Conversations.FindAsync(model.ConversationId);
            if (conversation == null || String.IsNullOrWhiteSpace(conversation.PhoneNumber))
            {
                return new ApiResult
                {
                    Message = "No conversation found"
                };
            }

            var interactiveTemplate = await _dbContext.InteractiveTemplates.FindAsync(model.InteractiveTemplateId);
            if (interactiveTemplate == null)
                return new ApiResult { Message = "Interactive template not found" };

            if (interactiveTemplate.ClientId != model.ClientId || interactiveTemplate.SenderId != model.SenderId)
                return new ApiResult { Message = "Interactive template does not belong to this sender id" };

            if (!(interactiveTemplate.UsedByAgent ?? false))
                return new ApiResult { Message = "Agent cannot use this template" };

            if (interactiveTemplate.DefaultTypeId > 0)
                return new ApiResult { Message = "Agent cannot use this template" };

            var messageReceived = new UMessageReceived
            {
                ModuleId = (int)ModuleEnum.Chat,
                ParentId = model.ConversationId,
                ActionId = model.InteractiveTemplateId,
                ActionType = 1
            };

            var result = await SendInteractiveMessageAsync(messageReceived, model.ClientId, model.SenderId, conversation.PhoneNumber, model.MediaId, model.Values);

            if (result == null)
                return new ApiResult { Message = "Cannot send message, please try again!" };

            if (result.Status <= 0)
                return new ApiResult { Message = result.Message };

            return new ApiResult
            {
                Success = true,
                StatusCode = 200,
                Message = result.Message
            };
        }
    }
}
