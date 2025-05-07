using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Entity;
using static WhatsAppAPISolutionDL.Dto.Flow.FlowResponseDto;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MediatorService : IMediatorService
    {
        #region Fields    

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<MediatorService> _logger;
        private readonly IInteractiveTemplateService _interactiveTemplateService;
        private readonly ICommunicationService _communicationService;
        private readonly IMessageSentLogsService _messageSentLogsService;
        private readonly IHubContext<ConversationHub> _conversationHubContext;
        private readonly IAgentsService _agentsService;
        private readonly IOneSignalService _oneSignalService;
        private readonly ILocationService _locationService;

        #endregion

        #region Ctor

        public MediatorService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<MediatorService> logger,
            IInteractiveTemplateService interactiveTemplateService,
            ICommunicationService communicationService,
            IMessageSentLogsService messageSentLogsService,
            IHubContext<ConversationHub> conversationHubContext,
            IAgentsService agentsService,
            IOneSignalService oneSignalService,
            ILocationService locationService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _interactiveTemplateService = interactiveTemplateService;
            _communicationService = communicationService;
            _messageSentLogsService = messageSentLogsService;
            _conversationHubContext = conversationHubContext;
            _agentsService = agentsService;
            _oneSignalService = oneSignalService;
            _locationService = locationService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Use to send message as well as interactive template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiResult> SendInteractiveTemplate(InteractiveMessageRequestDto model)
        {
            var result = await _communicationService.SendInteractiveMessageAsync(model);

            SendSmsResultDto sendSMSResult = null;
            if (result.Result != null && result.Result.GetType() == typeof(SendSmsResultDto))
                sendSMSResult = (SendSmsResultDto)result.Result;

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
                        foreach (var button in model.Buttons)
                        {
                            button.ButtonValue = button.ButtonValue.Replace(value.Key, value.Value);
                        }
                    }
                }
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


            //Create button json
            if (model.Buttons != null && model.Buttons.Any())
            {
                buttonJson = JsonConvert.SerializeObject(model.Buttons.Select(x => new ButtonDto
                {
                    ButtonId = x.ButtonId,
                    ButtonText = x.ButtonText,
                    ButtonValue = x.ButtonValue,
                    ButtonType = x.ButtonType ?? 0,
                    Sequence = x.Sequence ?? 0
                }).ToList());
            }

            var message = new InsertMessageDto
            {
                ClientId = model.ClientId,
                SenderId = model.SenderId,
                WaId = sendSMSResult != null ? sendSMSResult.waId : String.Empty,
                RecipientId = model.PhoneNumber,
                Status = (sendSMSResult != null && sendSMSResult.success) ? MessageStatusEnum.SENT : MessageStatusEnum.FAILED,
                ModuleId = model.ModuleId,
                ParentId = model.ParentId,
                MessageType = (int)MainMessageTypeEnum.INTERACTIVETEMPLATE,
                MessageReferenceId = model.ActionId,
                MessageContent = messageContent.ToString(),
                ButtonJson = buttonJson,
                MediaId = model.MediaId, //If in campaign media id is present take reference from there, else default media
            };

            if (sendSMSResult != null && sendSMSResult.errors != null && sendSMSResult.errors.Any())
            {
                string errors = String.Join(',', sendSMSResult.errors);
                message.Error = new InsertMessageDto.ErrorDto
                {
                    ErrorDetails = errors
                };
            }

            await _messageSentLogsService.AddMessageSentLogAsync(message);

            return result;
        }

        private async Task<string> SendContentToWitAi(string message)
        {
            string resultString = string.Empty;

            try
            {
                var client1 = new HttpClient();
                var apiUrl = $"https://api.wit.ai/message?v=20250405&q={message}";
                client1.DefaultRequestHeaders.Clear();
                client1.DefaultRequestHeaders.Add("Authorization", "Bearer XKNTI6I446CSPCAJ52VABGCSVPYS2QCI");

                var response1 = await client1.GetAsync(apiUrl);
                _logger.LogInformation("FlowResponseAsync - getting response from wit.ai response = {response}", JsonConvert.SerializeObject(response1));

                if (response1.IsSuccessStatusCode)
                {
                    var content = await response1.Content.ReadAsStringAsync();
                    _logger.LogInformation("FlowResponseAsync - getting response from wit.ai success response = {response}", JsonConvert.SerializeObject(content));
                    var result = JsonConvert.DeserializeObject<WitAiResponseDto>(content);
                    if (result != null)
                    {
                        _logger.LogInformation("FlowResponseAsync - getting response from wit.ai success DeserializeObject response = {response}", JsonConvert.SerializeObject(result));
                        if (result.Intents != null && result.Intents.Count > 0)
                        {
                            var intent = result.Intents[0];
                            resultString += $" - Intent: {intent.Name} ({intent.Confidence:F3})";
                        }
                        if (result.Traits?.WitSentiment != null && result.Traits.WitSentiment.Count > 0)
                        {
                            var sentiment = result.Traits.WitSentiment[0];
                            resultString += $" | Sentiment: {sentiment.Value} ({sentiment.Confidence:F3})";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return resultString;
        }

        private async Task SendSignalRNotification(int clientId, int senderId, int agentId, SignalREnum type, object data)
        {
            // Look up the connection ID for the Agent ID and send the conversation
            string connectionId = String.Empty;
            int i;
            int conversationId = 0;
            for (i = 1; i <= 5; i++)
            {
                if (ConversationHub.connections.TryGetValue(agentId, out connectionId))
                {
                    await _conversationHubContext.Clients.Client(connectionId).SendAsync(type.ToString(), data);

                    if (type == SignalREnum.MessageReceived && await _agentsService.IsAgentOneSignalEnabled(clientId, senderId))
                    {
                        if (data.GetType() == typeof(ULatestConversationByConversation))
                        {
                            var conversation = (ULatestConversationByConversation)data;
                            conversationId = conversation.Id ?? 0;

                            if (!String.IsNullOrWhiteSpace(conversation.MessageContent))
                            {
                                var result = await SendContentToWitAi(conversation.MessageContent);
                                conversation.MessageContent = String.Concat(conversation.MessageContent, result);
                                await _oneSignalService.SendMessageReceivedNotification(agentId, conversation.Language, conversation.MessageContent);
                            }
                        }
                    }

                    _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", type.ToString(), agentId, connectionId, conversationId, i, JsonConvert.SerializeObject(data));
                    break;
                }
                else
                    _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", type.ToString(), agentId, connectionId, conversationId, i, JsonConvert.SerializeObject(data));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Centralized function to process all type of DB response
        /// </summary>
        /// <param name="dBResponse"></param>
        /// <returns></returns>
        public async Task<ApiResult> ProcessDBResponse(int clientId, int senderId, DBResponse model)
        {
            _logger.LogInformation("Calling function ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse}", clientId, senderId, model);

            if (model == null || String.IsNullOrWhiteSpace(model.Json))
                return new ApiResult { Success = false, Message = $"DBResponse was null or json was empty. DBResponse={JsonConvert.SerializeObject(model)}" };

            switch (model.ResponseType)
            {
                case (int)DBResponseEnum.InteractiveTemplate:

                    //Parse DB response
                    var interactiveTemplateDBResponse = JsonConvert.DeserializeObject<InteractiveTemplateDBResponse>(model.Json);
                    if (interactiveTemplateDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, interactiveTemplateDBResponse);

                    //If action type > 0 and actionId > 0
                    if (interactiveTemplateDBResponse.ActionType > 0 && interactiveTemplateDBResponse.ActionId > 0 && interactiveTemplateDBResponse.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    {
                        if (String.IsNullOrWhiteSpace(interactiveTemplateDBResponse.FlowToken))
                            interactiveTemplateDBResponse.FlowToken = $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{interactiveTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{interactiveTemplateDBResponse.ParentId}";
                        else
                            interactiveTemplateDBResponse.FlowToken = String.Concat(interactiveTemplateDBResponse.FlowToken.TrimEnd('|'), "|", $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{interactiveTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{interactiveTemplateDBResponse.ParentId}");

                        var interactiveTemplate = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, interactiveTemplateDBResponse.ActionId);
                        if (interactiveTemplate == null)
                            return new ApiResult { Success = false, Message = $"Cannot find interactive template with ID={interactiveTemplateDBResponse.ActionId}" };

                        var interactiveMessageRequest = new InteractiveMessageRequestDto
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            ActionId = interactiveTemplateDBResponse.ActionId,
                            ModuleId = interactiveTemplateDBResponse.ModuleId,
                            ParentId = interactiveTemplateDBResponse.ParentId,
                            MessageReferenceId = interactiveTemplateDBResponse.ActionId,
                            PhoneNumber = interactiveTemplateDBResponse.PhoneNumber,
                            HeaderType = interactiveTemplate.HeaderType ?? 0,
                            HeaderText = interactiveTemplate.HeaderText,
                            BodyText = interactiveTemplate.BodyText,
                            FooterText = interactiveTemplate.FooterText,
                            MediaId = interactiveTemplate.MediaId ?? 0,
                            FlowToken = interactiveTemplateDBResponse.FlowToken
                        };

                        //Add dynamic parameters, if passed from DB
                        if (interactiveTemplateDBResponse.Params != null && interactiveTemplateDBResponse.Params != null)
                        {
                            foreach (var item in interactiveTemplateDBResponse.Params)
                            {
                                interactiveMessageRequest.Values.Add(new ParamValue
                                {
                                    Key = item.Key,
                                    Value = item.Value
                                });
                            }
                        }

                        //Add button from interactive template itself
                        if (interactiveTemplate.Buttons != null && interactiveTemplate.Buttons.Any())
                        {
                            foreach (var button in interactiveTemplate.Buttons)
                            {
                                interactiveMessageRequest.Buttons.Add(new InteractiveMessageRequestDto.Button
                                {
                                    ButtonId = Convert.ToString(button.ButtonId),
                                    ButtonText = button.ButtonText,
                                    ButtonType = button.ButtonType,
                                    ButtonValue = button.ButtonValue,
                                    Sequence = button.Sequence,
                                    ActionId = button.ActionId,
                                    ActionType = button.ActionType
                                });
                            }
                        }

                        var messageResult = await SendInteractiveTemplate(interactiveMessageRequest);
                    }

                    //If agent id is available, send signalR notification
                    if (interactiveTemplateDBResponse.ModuleId == (int)ModuleEnum.Chat
                        && interactiveTemplateDBResponse.ConversationMessageId > 0
                        && interactiveTemplateDBResponse.AgentId > 0
                        && interactiveTemplateDBResponse.IsFoul == 0)
                    {
                        var startProcTime = DateTime.UtcNow;
                        var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationByMessageId},@ClientId={clientId}, @SenderId={senderId}, @MessageId={interactiveTemplateDBResponse.ConversationMessageId}, @Status={(int)ConversationStatusEnum.AgentAssigned}").ToListAsync();
                        _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, senderId={senderId}, conversationMessageId={conversationMessageId}, status={status}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", clientId, senderId, interactiveTemplateDBResponse.ConversationMessageId, (int)ConversationStatusEnum.AgentAssigned, (int)CrudEnum.GetConversationByMessageId, CrudEnum.GetConversationByMessageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                        if (response.Any())
                            await SendSignalRNotification(clientId, senderId, interactiveTemplateDBResponse.AgentId, SignalREnum.MessageReceived, response[0]);
                    }

                    break;
                case (int)DBResponseEnum.ManualTemplate:

                    //Parse DB response
                    var manualTemplateDBResponse = JsonConvert.DeserializeObject<ManualTemplateDBResponse>(model.Json);
                    if (manualTemplateDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, manualTemplateDBResponse);

                    if (manualTemplateDBResponse.ActionType > 0 && manualTemplateDBResponse.ActionId > 0 && manualTemplateDBResponse.ActionType == (int)ActionTypeEnum.TEMPLATE)
                    {
                        if (String.IsNullOrWhiteSpace(manualTemplateDBResponse.FlowToken))
                            manualTemplateDBResponse.FlowToken = $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{manualTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{manualTemplateDBResponse.ParentId}";
                        else
                            manualTemplateDBResponse.FlowToken = String.Concat(manualTemplateDBResponse.FlowToken.TrimEnd('|'), "|", $"{FlowIdentifier.ClientId}:{clientId}|" + $"{FlowIdentifier.SenderId}:{senderId}|" + $"{FlowIdentifier.ModuleId}:{manualTemplateDBResponse.ModuleId}|" + $"{FlowIdentifier.ParentId}:{manualTemplateDBResponse.ParentId}");

                        var interactiveMessageRequest = new InteractiveMessageRequestDto
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            ActionId = manualTemplateDBResponse.ActionId,
                            ModuleId = manualTemplateDBResponse.ModuleId,
                            ParentId = manualTemplateDBResponse.ParentId,
                            MessageReferenceId = manualTemplateDBResponse.ActionId,
                            PhoneNumber = manualTemplateDBResponse.PhoneNumber,
                            HeaderType = manualTemplateDBResponse.HeaderType,
                            HeaderText = manualTemplateDBResponse.HeaderText,
                            BodyText = manualTemplateDBResponse.BodyText,
                            FooterText = manualTemplateDBResponse.FooterText,
                            MediaId = manualTemplateDBResponse.MediaId,
                            FlowToken = manualTemplateDBResponse.FlowToken
                        };

                        //Add dynamic parameters, if passed from DB
                        if (manualTemplateDBResponse.Params != null && manualTemplateDBResponse.Params != null)
                        {
                            foreach (var item in manualTemplateDBResponse.Params)
                            {
                                interactiveMessageRequest.Values.Add(new ParamValue
                                {
                                    Key = item.Key,
                                    Value = item.Value
                                });
                            }
                        }

                        //Add button from interactive template itself
                        if (manualTemplateDBResponse.Buttons != null && manualTemplateDBResponse.Buttons.Any())
                        {
                            foreach (var button in manualTemplateDBResponse.Buttons)
                            {
                                interactiveMessageRequest.Buttons.Add(new InteractiveMessageRequestDto.Button
                                {
                                    ButtonId = Convert.ToString(button.ButtonId),
                                    ButtonText = button.ButtonText,
                                    ButtonType = button.ButtonType,
                                    ButtonValue = button.ButtonValue,
                                    Sequence = button.Sequence,
                                    ActionId = button.ActionId,
                                    ActionType = button.ActionType
                                });
                            }
                        }

                        var messageResult = await SendInteractiveTemplate(interactiveMessageRequest);
                    }

                    //If agent id is available, send signalR notification
                    if (manualTemplateDBResponse.ModuleId == (int)ModuleEnum.Chat
                        && manualTemplateDBResponse.ConversationMessageId > 0
                        && manualTemplateDBResponse.AgentId > 0
                        && manualTemplateDBResponse.IsFoul == 0)
                    {
                        var startProcTime = DateTime.UtcNow;
                        var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationByMessageId},@ClientId={clientId}, @SenderId={senderId}, @MessageId={manualTemplateDBResponse.ConversationMessageId}, @Status={(int)ConversationStatusEnum.AgentAssigned}").ToListAsync();
                        _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, senderId={senderId}, conversationMessageId={conversationMessageId}, status={status}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", clientId, senderId, manualTemplateDBResponse.ConversationMessageId, (int)ConversationStatusEnum.AgentAssigned, (int)CrudEnum.GetConversationByMessageId, CrudEnum.GetConversationByMessageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                        if (response.Any())
                            await SendSignalRNotification(clientId, senderId, manualTemplateDBResponse.AgentId, SignalREnum.MessageReceived, response[0]);
                    }

                    break;
                case (int)DBResponseEnum.AddressCheck:

                    //Parse DB response
                    var addressCheckDBResponse = JsonConvert.DeserializeObject<ManualTemplateDBResponse>(model.Json);
                    if (addressCheckDBResponse == null)
                        return new ApiResult { Success = false, Message = $"Cannot parse DBResponse JSON. DBResponse={JsonConvert.SerializeObject(model)}" };

                    _logger.LogInformation("Parsed ProcessDBResponse with received clientId={clientId} senderId={senderId} and DBResponse={DBResponse} and result={result}", clientId, senderId, model, addressCheckDBResponse);

                    string geoLocation = String.Empty;
                    string orderId = String.Empty;

                    if (addressCheckDBResponse.KeyValues != null && addressCheckDBResponse.KeyValues.Any())
                    {
                        var geoLocationParam = addressCheckDBResponse.KeyValues.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.Key) && x.Key.Equals(DBResponseKey.GEOLOCATION, StringComparison.OrdinalIgnoreCase));
                        if (geoLocationParam != null)
                            geoLocation = geoLocationParam.Value;

                        var orderParam = addressCheckDBResponse.KeyValues.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x.Key) && x.Key.Equals(DBResponseKey.ORDERID, StringComparison.OrdinalIgnoreCase));
                        if (orderParam != null)
                            orderId = orderParam.Value;
                    }

                    //Call decima service
                    //Response
                    var deliveryStatus = await _locationService.GetDeliveryStatus(orderId, geoLocation);

                    //var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_SaveModifiers @FlowToken={flowResponse.flowResponse.flowToken},@Json={modifierItemsJson}").ToListAsync();
                    //_logger.LogInformation("Received response from procedure usp_Orders_SaveModifiers_Temp with OrderItemId={orderItemId} and ModifierItemsJson={json} and response={response}", orderItemId, JsonConvert.SerializeObject(modifierItemsJson), JsonConvert.SerializeObject(dbresponse));

                    //Call ProcessDBResponse(dbresponse);

                    break;
                case (int)DBResponseEnum.PaymentRequest:

                    //Call decima service
                    //Response

                    break;
                default:
                    break;
            }

            return new ApiResult { Success = true, Message = "Data successfully added" };
        }

        #endregion
    }
}
