using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
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
        private readonly ILogger<CommunicationService> _logger;

        public CommunicationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            ITemplateService templateService,
            IMessageSentLogsService messageSentLogsService,
            IInteractiveTemplateService interactiveTemplateService,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IMediaService mediaService,
            ILogger<CommunicationService> logger)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _templateService = templateService;
            _interactiveTemplateService = interactiveTemplateService;
            _messageSentLogsService = messageSentLogsService;
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _mediaService = mediaService;
            _logger = logger;
        }

        public async Task<ApiResult> SendTemplateMessageAsync(TemplateMessagePayloadDto model)
        {
            var template = await _templateService.GetTemplateDetailAsync(model.ClientId, model.TemplateId);
            if (template == null)
                return new ApiResult { StatusCode = 0, Message = "Template not found or deleted" };

            ParamData headerParameter = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault() : null;
            List<ParamData> bodyParameters = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList() : new List<ParamData>();
            List<ParamData> buttonParameters = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList() : new List<ParamData>();

            string headerText = template.HeaderText ?? "";
            string bodyText = template.BodyText ?? "";
            string footerText = template.FooterText ?? "";
            string buttonJson = String.Empty;

            var sendMessage = new SendTemplateMessageDto
            {
                ClientId = template.ClientId.ToString(),
                SenderNameId = template.SenderId.ToString(),
                PhoneNumbers = model.PhoneNumbers,
                LanguageCode = template.Language,
                TemplateId = template.TemplateId,
                TemplateName = template.TemplateName
            };

            #region Header 

            var headerType = (TemplateHeaderEnum)template.HeaderType;
            if (headerType == TemplateHeaderEnum.TEXT && template.HeaderParamCount > 0)
            {
                if (headerParameter == null || String.IsNullOrWhiteSpace(headerParameter.ParamValue))
                    return new ApiResult { StatusCode = 0, Message = $"error - HParam is required." };

                var headerComponents = new SendTemplateMessageDto.TemplateComponent
                {
                    ComponentType = nameof(TemplateParamEnum.Header)
                };

                headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue
                {
                    Type = headerType.ToString(),
                    Value = headerParameter.ParamValue,
                    Index = headerParameter.Sequence ?? 0,
                });

                //Replace {{example}} in header
                var templateHeaderParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                if (templateHeaderParam != null)
                    headerText = headerText.Replace(templateHeaderParam.ParamName, headerParameter.ParamValue);

                sendMessage.Components.Add(headerComponents);
            }
            else if (headerType == TemplateHeaderEnum.IMAGE || headerType == TemplateHeaderEnum.DOCUMENT || headerType == TemplateHeaderEnum.VIDEO)
            {
                var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                {
                    ComponentType = TemplateParamEnum.Header.ToString()
                };

                var media = _dbContext.Medias.Find(model.MediaId > 0 ? model.MediaId : template.MediaId); //If in campaign media id is present take reference from there, else default media
                if (media != null)
                {
                    var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = headerType.ToString(),
                        Value = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath
                    });
                }
                else
                    return new ApiResult { Message = $"error - Cannot find template media." };

                sendMessage.Components.Add(headerComponents);
            }

            #endregion

            #region Body

            if (template.BodyParamCount > 0)
            {
                if (bodyParameters.Count == 0 || bodyParameters.Count < template.BodyParamCount)
                    return new ApiResult { StatusCode = 0, Message = $"error - Body paramaters passed is less than required parameters." };

                var bodyComponents = new SendTemplateMessageDto.TemplateComponent
                {
                    ComponentType = TemplateParamEnum.Body.ToString()
                };

                for (var i = 0; i < template.BodyParamCount; i++)
                {
                    var param = bodyParameters.Where(x => x.Sequence == i).FirstOrDefault();
                    if (param == null || String.IsNullOrWhiteSpace(param.ParamValue))
                        return new ApiResult { StatusCode = 0, Message = $"error - BParam {i + 1} is not passed." };

                    //Replace {{example}} in body
                    var templateBodyParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body && x.Sequence == i).FirstOrDefault();
                    if (templateBodyParam != null)
                        bodyText = bodyText.Replace(templateBodyParam.ParamName, param.ParamValue);

                    bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = "text",
                        Value = param.ParamValue,
                        Index = i
                    });
                }

                sendMessage.Components.Add(bodyComponents);
            }

            #endregion

            #region Button

            //If FLOW type action is present, skip buttons
            if (template.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW))
            {
                var button = template.Buttons.FirstOrDefault(x => x.ActionType == (int)ActionTypeEnum.FLOW);
                var flow = await _dbContext.Flows.FindAsync(button.ActionId);
                if (flow == null)
                    return new ApiResult { StatusCode = 0, Message = $"Flow not found with id - {button.ActionId}" };

                if (String.IsNullOrWhiteSpace(flow.MetaFlowId))
                    return new ApiResult { StatusCode = 0, Message = $"Flow with id - {button.ActionId} does not have meta id yet" };

                if ((flow.Status ?? "").ToLower() != FlowStatusEnum.PUBLISHED.ToString().ToLower())
                    return new ApiResult { StatusCode = 0, Message = $"Flow is not published with id - {button.ActionId}" };

                sendMessage.FlowAction = new SendTemplateMessageDto.FlowActionDto
                {
                    Token = (model.FlowToken ?? "") + $"|{FlowIdentifier.FlowId}:{flow.FlowId}", //Append flow id for identification
                    Index = 0
                };
            }
            else
            {
                var templateButtonParams = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
                if (templateButtonParams != null && templateButtonParams.Any())
                {
                    var buttonComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Button.ToString()
                    };

                    for (var i = 0; i < templateButtonParams.Count; i++)
                    {
                        var param = buttonParameters.Where(x => x.Sequence == templateButtonParams[i].Sequence).FirstOrDefault();
                        if (param == null || String.IsNullOrWhiteSpace(param.ParamValue))
                            return new ApiResult { StatusCode = 0, Message = $"error - BtnParam {templateButtonParams[i].Sequence + 1} is not passed." };

                        buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                        {
                            Type = nameof(ButtonTypeEnum.URL),
                            Value = param.ParamValue,
                            Index = templateButtonParams[i].Sequence ?? 0
                        });
                    }

                    sendMessage.Components.Add(buttonComponents);
                }
            }

            //Add the button in button JSON
            List<ButtonDto> buttons = new List<ButtonDto>();
            if (template.Buttons != null && template.Buttons.Any())
            {
                for (int i = 0; i < template.Buttons.Count; i++)
                {
                    var button = template.Buttons[i];
                    string buttonValue = button.ButtonValue ?? "";

                    //Get the dynamic value
                    var buttonParam = buttonParameters.Where(x => x.Sequence == button.Sequence).FirstOrDefault();

                    //Replace {{example}} in button
                    var templateButtonParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button && x.Sequence == button.Sequence).FirstOrDefault();
                    if (templateButtonParam != null && buttonParam != null)
                        buttonValue = buttonValue.Replace(templateButtonParam.ParamName, buttonParam.ParamValue);

                    buttons.Add(new ButtonDto
                    {
                        ButtonId = Convert.ToString(button.ButtonId),
                        ButtonText = button.ButtonText,
                        ButtonValue = buttonValue,
                        ButtonType = button.ButtonType ?? 0,
                        Sequence = button.Sequence ?? 0
                    });
                }
            }

            #endregion

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
                messageContent.Append(footerText);

            buttonJson = JsonConvert.SerializeObject(buttons);

            var request = JsonConvert.SerializeObject(sendMessage);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Template/SendBatchTemplateMessage";

            var res = new StringContent(request, Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync(apiEndpoint, res);
            var content = await response1.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} Send Template Message with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

            List<CustomIntegrationResult> models = new List<CustomIntegrationResult>();
            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    foreach (var item in tempResult)
                    {
                        var response = new CustomIntegrationResult
                        {
                            Sent = item.success,
                            PhoneNumber = item.phoneNumber,
                            WaId = item.waId
                        };

                        var message = new InsertMessageDto
                        {
                            ClientId = template.ClientId ?? 0,
                            SenderId = template.SenderId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId,
                            ParentId = model.ParentId,
                            MessageType = (int)MainMessageTypeEnum.TEMPLATE,
                            MessageReferenceId = (int)template.Id,
                            MessageContent = messageContent.ToString(),
                            ButtonJson = buttonJson,
                            MediaId = (model.MediaId > 0 ? model.MediaId : template.MediaId) ?? 0, //If in campaign media id is present take reference from there, else default media
                            UDF1 = model.UDF1,
                            UDF2 = model.UDF2,
                            
                        };

                        if (item.errors != null && item.errors.Any())
                        {
                            string errors = String.Join(',', item.errors);
                            message.Error = new InsertMessageDto.ErrorDto
                            {
                                ErrorDetails = errors
                            };

                            response.Errors = errors;
                        }

                        await _messageSentLogsService.AddMessageSentLogAsync(message);

                        //Add result to custom integration result models
                        models.Add(response);
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

        public async Task<ApiResult> SendCarouselTemplateMessageAsync(TemplateMessagePayloadDto model)
        {
            var template = await _templateService.GetTemplateDetailAsync(model.ClientId, model.TemplateId);
            if (template == null)
                return new ApiResult { StatusCode = 0, Message = "Template not found or deleted" };

            ParamData headerParameter = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault() : null;
            List<ParamData> bodyParameters = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList() : new List<ParamData>();
            List<ParamData> buttonParameters = model.Params != null ? model.Params.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList() : new List<ParamData>();

            string headerText = template.HeaderText ?? "";
            string bodyText = template.BodyText ?? "";
            string footerText = template.FooterText ?? "";
            string buttonJson = String.Empty;

            var sendMessage = new SendCarouselTemplateMessageDto
            {
                ClientId = template.ClientId.ToString(),
                SenderNameId = template.SenderId.ToString(),
                PhoneNumbers = model.PhoneNumbers,
                LanguageCode = template.Language,
                TemplateId = template.TemplateId,
                TemplateName = template.TemplateName,
                Cards = new List<SendCarouselTemplateMessageDto.CardComponent>()
            };

            foreach (var screen in template.Screens)
            {
                var cardComponent = new SendCarouselTemplateMessageDto.CardComponent
                {
                    Index = screen.Sequence ?? 0,
                    Components = new List<SendCarouselTemplateMessageDto.Component>()
                };

                #region Header 

                var headerType = (TemplateHeaderEnum)screen.HeaderType;
                if (headerType == TemplateHeaderEnum.TEXT && screen.HeaderParamCount > 0)
                {
                    if (headerParameter == null || String.IsNullOrWhiteSpace(headerParameter.ParamValue))
                        return new ApiResult { StatusCode = 0, Message = $"error - HParam is required." };

                    var headerComponents = new SendCarouselTemplateMessageDto.Component
                    {
                        ComponentType = nameof(TemplateParamEnum.Header)
                    };

                    headerComponents.Values.Add(new SendCarouselTemplateMessageDto.KeyValue
                    {
                        Type = headerType.ToString(),
                        Value = headerParameter.ParamValue,
                        Index = headerParameter.Sequence ?? 0,
                    });

                    //Replace {{example}} in header
                    var templateHeaderParam = template.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                    if (templateHeaderParam != null)
                        headerText = headerText.Replace(templateHeaderParam.ParamName, headerParameter.ParamValue);

                    cardComponent.Components.Add(headerComponents);
                }
                else if (headerType == TemplateHeaderEnum.IMAGE || headerType == TemplateHeaderEnum.DOCUMENT || headerType == TemplateHeaderEnum.VIDEO)
                {
                    var headerComponents = new SendCarouselTemplateMessageDto.Component
                    {
                        ComponentType = TemplateParamEnum.Header.ToString()
                    };

                    var media = _dbContext.Medias.Find(model.MediaId > 0 ? model.MediaId : screen.MediaId); //If in campaign media id is present take reference from there, else default media
                    if (media != null)
                    {
                        var mediaPath = String.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                        headerComponents.Values.Add(new SendCarouselTemplateMessageDto.KeyValue
                        {
                            Type = headerType.ToString(),
                            Value = !String.IsNullOrWhiteSpace(media.MediaId) ? media.MediaId : mediaPath
                        });
                    }
                    else
                        return new ApiResult { Message = $"error - Cannot find template media." };

                    cardComponent.Components.Add(headerComponents);
                }

                #endregion

                #region Body

                if (screen.BodyParamCount > 0)
                {
                    if (bodyParameters.Count == 0 || bodyParameters.Count < screen.BodyParamCount)
                        return new ApiResult { StatusCode = 0, Message = $"error - Body paramaters passed is less than required parameters." };

                    var bodyComponents = new SendCarouselTemplateMessageDto.Component
                    {
                        ComponentType = TemplateParamEnum.Body.ToString()
                    };

                    for (var i = 0; i < screen.BodyParamCount; i++)
                    {
                        var param = bodyParameters.Where(x => x.Sequence == i).FirstOrDefault();
                        if (param == null || String.IsNullOrWhiteSpace(param.ParamValue))
                            return new ApiResult { StatusCode = 0, Message = $"error - BParam {i + 1} is not passed." };

                        //Replace {{example}} in body
                        var templateBodyParam = template.Parameters.Where(x => x.TemplateScreenId == screen.TemplateScreenId && x.ParamType == (int)TemplateParamEnum.Body && x.Sequence == i).FirstOrDefault();
                        if (templateBodyParam != null)
                            bodyText = bodyText.Replace(templateBodyParam.ParamName, param.ParamValue);

                        bodyComponents.Values.Add(new SendCarouselTemplateMessageDto.KeyValue
                        {
                            Type = "text",
                            Value = param.ParamValue,
                            Index = i
                        });
                    }

                    cardComponent.Components.Add(bodyComponents);
                }

                #endregion

                #region Button

                var templateButtonParams = template.Parameters.Where(x => x.TemplateScreenId == screen.TemplateScreenId && x.ParamType == (int)TemplateParamEnum.Button).ToList();
                if (templateButtonParams != null && templateButtonParams.Any())
                {
                    var buttonComponents = new SendCarouselTemplateMessageDto.Component
                    {
                        ComponentType = TemplateParamEnum.Button.ToString()
                    };

                    for (var i = 0; i < templateButtonParams.Count; i++)
                    {
                        var param = buttonParameters.Where(x => x.Sequence == templateButtonParams[i].Sequence).FirstOrDefault();
                        if (param == null || String.IsNullOrWhiteSpace(param.ParamValue))
                            return new ApiResult { StatusCode = 0, Message = $"error - BtnParam {templateButtonParams[i].Sequence + 1} is not passed." };

                        buttonComponents.Values.Add(new SendCarouselTemplateMessageDto.KeyValue
                        {
                            Type = nameof(ButtonTypeEnum.URL),
                            Value = param.ParamValue,
                            Index = templateButtonParams[i].Sequence ?? 0
                        });
                    }

                    cardComponent.Components.Add(buttonComponents);
                }

                #endregion

                sendMessage.Cards.Add(cardComponent);
            }

            //Add the button in button JSON
            List<ButtonDto> buttons = new List<ButtonDto>();

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
                messageContent.Append(footerText);

            buttonJson = JsonConvert.SerializeObject(buttons);

            var request = JsonConvert.SerializeObject(sendMessage);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Template/SendBatchCarouselMessage";

            var res = new StringContent(request, Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync(apiEndpoint, res);
            var content = await response1.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} SendBatchCarouselMessage with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

            List<CustomIntegrationResult> models = new List<CustomIntegrationResult>();
            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var tempResult = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (tempResult != null)
                {
                    foreach (var item in tempResult)
                    {
                        var response = new CustomIntegrationResult
                        {
                            Sent = item.success,
                            PhoneNumber = item.phoneNumber,
                            WaId = item.waId
                        };

                        var message = new InsertMessageDto
                        {
                            ClientId = template.ClientId ?? 0,
                            SenderId = template.SenderId,
                            WaId = item.waId,
                            RecipientId = item.phoneNumber,
                            Status = item.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                            ModuleId = model.ModuleId,
                            ParentId = model.ParentId,
                            MessageType = (int)MainMessageTypeEnum.TEMPLATE,
                            MessageReferenceId = (int)template.Id,
                            MessageContent = messageContent.ToString(),
                            ButtonJson = buttonJson,
                            MediaId = (model.MediaId > 0 ? model.MediaId : template.MediaId) ?? 0 
                        };

                        if (item.errors != null && item.errors.Any())
                        {
                            string errors = String.Join(',', item.errors);
                            message.Error = new InsertMessageDto.ErrorDto
                            {
                                ErrorDetails = errors
                            };

                            response.Errors = errors;
                        }

                        await _messageSentLogsService.AddMessageSentLogAsync(message);

                        //Add result to custom integration result models
                        models.Add(response);
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

        public async Task<ApiResult> SendMessageAsync(SendMessageRequestDto model)
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
                    return new ApiResult { Success = false, Message = "No media found with this MediaId" };
            }

            var req = new SendMessageToBridgeDto
            {
                ClientId = model.ClientId.ToString(),
                SenderNameId = model.SenderId.ToString(),
                Type = ((MessageTypeEnum)model.Type).ToString(),
                Message = model.MessageContent,
                MediaId = mediaId,
                FileName = model.FileName,
                PhoneNumbers = model.PhoneNumbers,
            };

            var request = JsonConvert.SerializeObject(req);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Message/SendBatchMessage";

            var requestStr = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiEndpoint, requestStr);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} SendBatchMessage with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var sendSmsResults = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (sendSmsResults != null && sendSmsResults.Any())
                {
                    var sendSmsResult = sendSmsResults[0];
                    bool success = sendSmsResult.errors == null || !sendSmsResult.errors.Any();
                    return new ApiResult
                    {
                        Success = success,
                        StatusCode = success ? 200 : 0,
                        Result = sendSmsResult,
                        Message = sendSmsResult.errors != null && sendSmsResult.errors.Any() ? String.Join(',', sendSmsResult.errors) : "Message sent successfully"
                    };
                }
            }
            else if (result != null && !result.success)
            {
                return new ApiResult
                {
                    StatusCode = 0,
                    Success = false,
                    Message = result.message
                };
            }

            return new ApiResult
            {
                StatusCode = 200,
                Success = true,
                Message = "Message Sent Successfully"
            };
        }

        public async Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, int senderId, string phoneNumber, int mediaId = 0, List<ParamValue> values = null, string flowToken = "")
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
            string buttonJson = String.Empty;

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
                        foreach (var button in interactiveTemplate.Buttons.Where(x => !String.IsNullOrWhiteSpace(x.ButtonValue)))
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
                messageContent.Append(footerText);

            //In interactive button is required, if not available send normal message
            if (interactiveTemplate.Buttons == null || !interactiveTemplate.Buttons.Any() && !String.IsNullOrWhiteSpace(interactiveTemplate.BodyText))
            {
                var messageSentResult = await SendMessageAsync(new SendMessageRequestDto
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

                var sendMessageResponse = (SendSmsResultDto)messageSentResult.Result;

                var message = new InsertMessageDto
                {
                    ClientId = interactiveTemplate.ClientId ?? 0,
                    SenderId = interactiveTemplate.SenderId ?? 0,
                    WaId = sendMessageResponse != null ? sendMessageResponse.waId : String.Empty,
                    RecipientId = phoneNumber,
                    Status = sendMessageResponse.success ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                    ModuleId = model.ModuleId ?? 0,
                    ParentId = model.ParentId ?? 0,
                    MessageReferenceId = interactiveTemplate.Id,
                    MessageType = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                    MessageContent = messageContent.ToString(),
                    MediaId = media != null ? media.Id : 0,
                    Error = sendMessageResponse.errors != null && sendMessageResponse.errors.Any() ?
                            new InsertMessageDto.ErrorDto { ErrorDetails = String.Join(',', sendMessageResponse.errors) } : null
                };

                await _messageSentLogsService.AddMessageSentLogAsync(message);

                return new UResponse { Status = messageSentResult.StatusCode, Message = messageSentResult.Message };
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

                //If FLOW type action is present, skip buttons
                if (interactiveTemplate.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW))
                {
                    var button = interactiveTemplate.Buttons.FirstOrDefault(x => x.ActionType == (int)ActionTypeEnum.FLOW);
                    var flow = await _dbContext.Flows.FindAsync(button.ActionId);
                    if (flow == null)
                        return new UResponse { Status = 0, Message = $"Flow not found with id - {button.ActionId}" };

                    if (String.IsNullOrWhiteSpace(flow.MetaFlowId))
                        return new UResponse { Status = 0, Message = $"Flow with id - {button.ActionId} does not have meta id yet" };

                    if ((flow.Status ?? "").ToLower() != FlowStatusEnum.PUBLISHED.ToString().ToLower())
                        return new UResponse { Status = 0, Message = $"Flow is not published with id - {button.ActionId}" };

                    sendMessage.FlowAction = new SendInteractiveMessageRequestDto.FlowActionDto
                    {
                        FlowId = flow.MetaFlowId,
                        Version = "3", //flow.DataApiVersion, //must be 3 //https://developers.facebook.com/docs/whatsapp/flows/guides/sendingaflow/
                        ButtonText = button.ButtonText,
                        Token = (flowToken ?? "") + $"|{FlowIdentifier.FlowId}:{flow.FlowId}" //Append flow id for identification
                    };
                }
                else
                {
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

                //Create button json
                buttonJson = JsonConvert.SerializeObject(interactiveTemplate.Buttons.Select(x => new ButtonDto
                {
                    ButtonId = Convert.ToString(x.ButtonId),
                    ButtonText = x.ButtonText,
                    ButtonValue = x.ButtonValue,
                    ButtonType = x.ButtonType ?? 0,
                    Sequence = x.Sequence ?? 0
                }).ToList());
            }

            var request = JsonConvert.SerializeObject(sendMessage);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Message/SendInteractiveMessage";

            var requestStr = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiEndpoint, requestStr);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} SendInteractiveMessage with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

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

                        if (!item.success)
                            return new UResponse
                            {
                                Status = 0,
                                Message = item.errors != null && item.errors.Count() > 0 ? String.Join(',', item.errors) : "Something went wrong"
                            };
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

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Message/SendBatchMessage";

            var response = await _httpClient.PostAsync(apiEndpoint, new StringContent(requestStr, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} SendBatchMessage with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

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
            var interactiveTemplateParams = await _dbContext.InteractiveTemplateParameters.Where(x => x.InteractiveTemplateId == model.InteractiveTemplateId).ToListAsync();
            if (interactiveTemplate == null || interactiveTemplate.RecordStatus == -1) //If deleted
                return new ApiResult { Message = "Interactive template not found" };

            if (interactiveTemplate.ClientId != model.ClientId || interactiveTemplate.SenderId != model.SenderId)
                return new ApiResult { Message = "Interactive template does not belong to this sender id" };

            if (!(interactiveTemplate.UsedByAgent ?? false))
                return new ApiResult { Message = "Agent cannot use this template" };

            if (interactiveTemplate.DefaultTypeId > 0)
                return new ApiResult { Message = "Agent cannot use this template" };

            if (interactiveTemplateParams.Count > 0 && (model.Values == null || interactiveTemplateParams.Count != model.Values.Count))
                return new ApiResult { Message = $"Parameter(s) sent does not match with required parameters: {interactiveTemplateParams.Count}" };

            var messageReceived = new UMessageReceived
            {
                ModuleId = (int)ModuleEnum.Chat,
                ParentId = model.ConversationId,
                ActionId = model.InteractiveTemplateId,
                ActionType = 1
            };

            var flowToken = $"{FlowIdentifier.ClientId}:{model.ClientId}|" + $"{FlowIdentifier.SenderId}:{model.SenderId}|" + $"{FlowIdentifier.ModuleId}:{(int)ModuleEnum.Chat}|" + $"{FlowIdentifier.ParentId}:{model.ConversationId}";
            var result = await SendInteractiveMessageAsync(messageReceived, model.ClientId, model.SenderId, conversation.PhoneNumber, model.MediaId, model.Values, flowToken: flowToken);
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

        /// <summary>
        /// This will just convert the provided object to bridge template dto and sends the response of message sent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApiResult> SendInteractiveMessageAsync(InteractiveMessageRequestDto model)
        {
            _logger.LogInformation("Calling SendInteractiveMessageAsync with model={model}", JsonConvert.SerializeObject(model));

            Media media = null;
            var headerType = (TemplateHeaderEnum)model.HeaderType;

            //Try to fetch the media
            if (headerType == TemplateHeaderEnum.IMAGE
                || headerType == TemplateHeaderEnum.VIDEO
                || headerType == TemplateHeaderEnum.DOCUMENT) //Try to get the media
                media = await _dbContext.Medias.FindAsync(model.MediaId > 0 ? model.MediaId : model.MediaId);

            string headerText = model.HeaderText ?? "";
            string bodyText = model.BodyText ?? "";
            string footerText = model.FooterText ?? "";
            string buttonJson = String.Empty;

            //Replace all the dynamic values with the correct one
            if (model.Values != null && model.Values.Any())
            {
                foreach (var value in model.Values)
                {
                    headerText = headerText.Replace(value.Key, value.Value);
                    bodyText = bodyText.Replace(value.Key, value.Value);
                    footerText = footerText.Replace(value.Key, value.Value);

                    if (model.Buttons != null && model.Buttons.Any())
                    {
                        foreach (var button in model.Buttons.Where(x => !String.IsNullOrWhiteSpace(x.ButtonValue)))
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
                messageContent.Append(footerText);

            //In interactive button is required or ask for location, if not available send normal message
            if (model.Buttons == null || !model.Buttons.Any() && !String.IsNullOrWhiteSpace(model.BodyText))
            {
                return await SendMessageAsync(new SendMessageRequestDto
                {
                    ClientId = model.ClientId,
                    SenderId = model.SenderId,
                    MediaId = media != null ? media.Id : 0,
                    ModuleId = model.ModuleId,
                    ParentId = model.ParentId,
                    ActionId = model.ActionId,
                    Type = type,
                    MessageTypeId = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                    MessageReferenceId = model.MessageReferenceId,
                    MessageContent = messageContent.ToString(),
                    FileName = media != null ? media.FileName : String.Empty,
                    PhoneNumbers = new List<string> { model.PhoneNumber }.TrimPhoneNumbers()
                });
            }

            var sendMessage = new SendInteractiveMessageRequestDto
            {
                ClientId = Convert.ToString(model.ClientId),
                SenderNameId = Convert.ToString(model.SenderId),
                PhoneNumbers = new List<string> { model.PhoneNumber }.TrimPhoneNumbers()
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

            if (model.Buttons != null && model.Buttons.Any())
            {
                sendMessage.Buttons = new List<SendInteractiveMessageRequestDto.ButtonDto>();

                //If Location type
                if (model.Buttons.Any(x => x.ButtonType == (int)ButtonTypeEnum.LOCATION))
                {
                    sendMessage.AskForLocation = true;
                }
                //If FLOW type action is present, skip buttons
                else if (model.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW))
                {
                    var button = model.Buttons.FirstOrDefault(x => x.ActionType == (int)ActionTypeEnum.FLOW);
                    var flow = await _dbContext.Flows.FindAsync(button.ActionId);
                    if (flow == null)
                        return new ApiResult { StatusCode = 0, Message = $"Flow not found with id - {button.ActionId}" };

                    if (String.IsNullOrWhiteSpace(flow.MetaFlowId))
                        return new ApiResult { StatusCode = 0, Message = $"Flow with id - {button.ActionId} does not have meta id yet" };

                    if ((flow.Status ?? "").ToLower() != FlowStatusEnum.PUBLISHED.ToString().ToLower())
                        return new ApiResult { StatusCode = 0, Message = $"Flow is not published with id - {button.ActionId}" };

                    var (isValid, path, matchedKeys) = (model.FlowToken ?? "").ParseIdPath<FlowTokenIdentifier>();
                    if (!matchedKeys.Contains(nameof(FlowTokenIdentifier.FlowId)))
                        model.FlowToken = model.FlowToken + $"|{FlowIdentifier.FlowId}:{flow.FlowId}";

                    sendMessage.FlowAction = new SendInteractiveMessageRequestDto.FlowActionDto
                    {
                        FlowId = flow.MetaFlowId,
                        Version = "3", //flow.DataApiVersion, //must be 3 //https://developers.facebook.com/docs/whatsapp/flows/guides/sendingaflow/
                        ButtonText = button.ButtonText,
                        Token = model.FlowToken ?? ""
                    };
                }
                else
                {
                    for (int i = 0; i < model.Buttons.Count; i++)
                    {
                        var button = model.Buttons[i];
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

                //Create button json
                buttonJson = JsonConvert.SerializeObject(model.Buttons.Select(x => new ButtonDto
                {
                    ButtonId = x.ButtonId,
                    ButtonText = x.ButtonText,
                    ButtonValue = x.ButtonValue,
                    ButtonType = x.ButtonType ?? 0,
                    Sequence = x.Sequence ?? 0
                }).ToList());
            }

            var request = JsonConvert.SerializeObject(sendMessage);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/Message/SendInteractiveMessage";

            var requestStr = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiEndpoint, requestStr);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} SendInteractiveMessage with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

            var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                var sendSmsResults = JsonConvert.DeserializeObject<List<SendSmsResultDto>>(data);
                if (sendSmsResults != null && sendSmsResults.Any())
                {
                    var sendSmsResult = sendSmsResults[0];
                    bool success = sendSmsResult.errors == null || !sendSmsResult.errors.Any();
                    return new ApiResult
                    {
                        Success = success,
                        StatusCode = success ? 200 : 0,
                        Result = sendSmsResult,
                        Message = sendSmsResult.errors != null && sendSmsResult.errors.Any() ? String.Join(',', sendSmsResult.errors) : "Message sent successfully"
                    };
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

            return new ApiResult { StatusCode = 0, Message = "Something went wrong while sending message" };
        }
    }
}
