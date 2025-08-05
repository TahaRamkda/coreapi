using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MessageService : IMessageService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IMediaService _mediaService;
        private readonly ICommunicationService _communicationService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<MessageService> _logger;
        private readonly IHubContext<ConversationHub> _conversationHubContext;
        private readonly IOneSignalService _oneSignalService;
        private readonly IAgentsService _agentsService;
        private readonly IMediatorService _mediatorService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IClientService _clientService;
        private readonly ISenderNameService _senderNameService;
        private readonly IUserService _userService;
        private readonly int ClientId;

        public MessageService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IMediaService mediaService,
            ICommunicationService communicationService,
            ILogger<MessageService> logger,
            IHubContext<ConversationHub> conversationHubContext,
            IConversationService conversationService,
            IOneSignalService oneSignalService,
            IAgentsService agentsService,
            IMediatorService mediatorService,
            IAppSettingsService appSettingsService,
            IClientService clientService,
            ISenderNameService senderNameService,
            IUserService userService
            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _mediaService = mediaService;
            _communicationService = communicationService;
            _logger = logger;
            _conversationHubContext = conversationHubContext;
            _conversationService = conversationService;
            _oneSignalService = oneSignalService;
            _agentsService = agentsService;
            _mediatorService = mediatorService;
            _appSettingsService = appSettingsService;
            _clientService = clientService;
            _senderNameService = senderNameService;
            _userService = userService;
            ClientId = _userService.GetClientIdFromAccessToken();
        }

        #region Utilities

        private async Task<bool> IsFoulMessage(int clientId, int senderId = 0, string msg = "")
        {
            if (String.IsNullOrWhiteSpace(msg))
                return false;

            string keyName = AppSettingKey.FoulLanguageWords;

            var appSetting = await _appSettingsService.GetAppSettingByKeyAsync(clientId, senderId, keyName);
            if (appSetting == null || String.IsNullOrWhiteSpace(appSetting.Val))
                return false;

            // Parse the foul words and clean up any whitespace
            var foulWords = appSetting.Val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(word => word.Trim()).Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();

            // Check for whole-word matches in the message using LINQ and Regex
            return foulWords.Any(word => System.Text.RegularExpressions.Regex.IsMatch(msg, $@"\b{System.Text.RegularExpressions.Regex.Escape(word)}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        #endregion

        public async Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus)
        {
            var messageStatusEnum = (MessageStatusEnum)Enum.Parse(typeof(MessageStatusEnum), messageStatus.status.ToUpper());

            if (messageStatusEnum == MessageStatusEnum.SENT)
                messageStatusEnum = MessageStatusEnum.SENTWA;

            var eventType = (int)messageStatusEnum;
            var eventStatus = messageStatusEnum == MessageStatusEnum.FAILED ? 0 : 1;
            var conversationId = "";
            var eventMessage = "";
            var pricingModel = "";
            var type = "";
            var billable = false;
            var category = "";
            int senderId = 0;

            if (messageStatus.phone_number_Id != null
                && !String.IsNullOrEmpty(messageStatus.phone_number_Id.display_phone_number)
                && !String.IsNullOrEmpty(messageStatus.phone_number_Id.phone_number_id))
            {
                var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberIdAsync(messageStatus.phone_number_Id.phone_number_id);
                if (senderName != null)
                    senderId = senderName.SenderId;
            }

            if (messageStatus.conversation != null)
                conversationId = messageStatus.conversation.id;

            if (messageStatus.error != null)
                eventMessage = messageStatus.error.error_Details;

            if (messageStatus.pricing != null)
            {
                pricingModel = messageStatus.pricing.pricing_model;
                billable = messageStatus.pricing.billable;
                category = messageStatus.pricing.category;
                type = messageStatus.pricing.type;
            }
            var startProcTime = DateTime.UtcNow;
             var response = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={0}, @ClientId={messageStatus.client_Id}, @ParentId={0}, @SenderId={senderId}, @PhoneNumber={messageStatus.recipient_Id}, @WaId={messageStatus.wam_Id}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={messageStatus.update_dateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}").ToListAsync();
            if (response.Any())
            {
                await _mediatorService.ProcessDBResponse(ClientId, senderId, response[0]);
            }
            _logger.LogDebug("Calling procedure usp_MessageSentLogs_StatusUpdate with ProcResponseTime={ProcResponseTime} ", DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return new UResponse
            {
                Status = 1,
                Message = "success"

            };
        }

        /// <summary>
        /// Add message received logs
        /// </summary>
        /// <param name="messageReceive"></param>
        /// <returns></returns>
        public async Task<ApiResult> AddMessageReceivedLogAsync(WhatsAppMessageReceiveDto messageReceive)
        {
            var client = await _clientService.GetClientEntityByIdAsync(Convert.ToInt32(messageReceive.client_Id));
            var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberIdAsync(messageReceive.phone_number_Id.phone_number_id);

            int messageType = 0;
            string messageText = String.Empty;
            int mediaId = 0;
            messageReceive.type = messageReceive.type.ToUpper();
            string fullName = messageReceive.contact != null ? messageReceive.contact.name : String.Empty;
            bool isFoulMsg = false;

            if (messageReceive.type == MessageReceiveTypeEnum.BUTTON.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; // Convert.ToInt32(MessageReceiveTypeEnum.BUTTON);
                messageText = messageReceive.button.payload ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.TEXT.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; //Convert.ToInt32(MessageReceiveTypeEnum.TEXT);
                messageText = messageReceive.text.body ?? "";
                isFoulMsg = await IsFoulMessage(client.ClientId, senderName?.SenderId ?? 0, messageText);
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.REACTION.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; //Convert.ToInt32(MessageReceiveTypeEnum.TEXT);
                messageText = messageReceive.reaction.emoji ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.IMAGE.ToString())
            {
                messageType = (int)MainMessageTypeEnum.MEDIA; // Convert.ToInt32(MessageReceiveTypeEnum.IMAGE);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.image.id);
                messageText = messageReceive.image.caption ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.VIDEO.ToString())
            {
                messageType = (int)MainMessageTypeEnum.MEDIA; // Convert.ToInt32(MessageReceiveTypeEnum.VIDEO);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.video.id);
                messageText = messageReceive.video.caption ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.AUDIO.ToString())
            {
                messageType = (int)MainMessageTypeEnum.MEDIA; // Convert.ToInt32(MessageReceiveTypeEnum.VIDEO);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.audio.id);
                messageText = "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.DOCUMENT.ToString())
            {
                messageType = (int)MainMessageTypeEnum.MEDIA; // Convert.ToInt32(MessageReceiveTypeEnum.DOCUMENT);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.document.id);
                messageText = messageReceive.document.caption ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.LOCATION.ToString())
            {
                messageType = (int)MainMessageTypeEnum.LOCATION;// Convert.ToInt32(MessageReceiveTypeEnum.LOCATION);
                messageText = String.Concat(messageReceive.location.latitude, ",", messageReceive.location.longitude);
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.STICKER.ToString())
            {
                messageType = (int)MainMessageTypeEnum.MEDIA; // Convert.ToInt32(MessageReceiveTypeEnum.STICKER);
                mediaId = await _mediaService.DownloadWhatsAppMediaToLocal(client, senderName, messageReceive.sticker.id);
                messageText = messageReceive.sticker.caption ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.INTERACTIVE.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; // Convert.ToInt32(MessageReceiveTypeEnum.INTERACTIVE);
                if (messageReceive.buttonReply != null)
                {
                    messageText = messageReceive.buttonReply.title ?? "";
                }
                else if (messageReceive.listReply != null)
                {
                    messageText = messageReceive.listReply.title ?? "";
                }
            }

            _logger.LogInformation("Calling Db procedure usp_MessageReceivedLogs_ops with request={request}", $"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id},@Name={fullName}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}, @IsFoul={isFoulMsg}");

            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id},@Name={fullName}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}, @IsFoul ={isFoulMsg}").ToListAsync();

            _logger.LogInformation("Calling procedure usp_MessageReceivedLogs_ops with request={request} and response={response} and ProcResponseTime={ProcResponseTime}", $"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id},@Name={fullName}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}, @IsFoul={isFoulMsg}", JsonConvert.SerializeObject(response), DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            //Mediator service
            if (response != null & response.Any())
            {
                _logger.LogDebug("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                var action = response[0];
                var result = await _mediatorService.ProcessDBResponse(client.ClientId, senderName.SenderId, action);
                return result;
            }

            return new ApiResult { Message = "Something went wrong" };
        }

        public async Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model)
        {
            return await _communicationService.SendAgentMessageAsync(model);
        }

        public async Task<ApiResult> SendAgentInteractiveMessageAsync(SendAgentInteractiveMessageRequestDto model)
        {
            return await _communicationService.SendAgentInteractiveMessageAsync(model);
        }

        public async Task<ApiResult> SaveSurveyResponse(FlowResponseDto flowResponse, Flow flow)
        {
            try
            {
                int parentId = 0;
                var (isValid, path, matchedKeys) = flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>();
                if (matchedKeys.Contains(nameof(FlowTokenIdentifier.ParentId)))
                    parentId = Convert.ToInt32(flowResponse.flowResponse.flowToken.ParseIdPath<FlowTokenIdentifier>().path.ParentId);

                var surveyResponse = new SurveyResponse
                {
                    SurveyId = flow.ParentId,
                    FlowId = flow.FlowId,
                    MetaFlowId = flow.MetaFlowId,
                    PhoneNumber = flowResponse.from,
                    Name = flow.FlowName ?? "Unknown",
                    FlowToken = flowResponse.flowResponse.flowToken,
                    SenderId = flow.SenderId ?? 0,
                    ClientId = flow.ClientId ?? 0,
                    ParentId = parentId,
                    ModuleId = (int)ModuleEnum.Survey,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.SurveyResponses.Add(surveyResponse);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("FlowResponseAsync - added response in SurveyResponse table with data = {data}", JsonConvert.SerializeObject(surveyResponse));

                // Prepare SurveyResponseDetails in a batch insert
                var surveyResponseDetails = flowResponse.flowResponse.responses
                    ?.SelectMany(response => response.multiSelect.Any()
                        ? response.multiSelect.Select(option => new SurveyResponseDetail
                        {
                            SurveyResponseId = surveyResponse.SurveyResponseId,
                            OptionText = option.Trim(),
                            QuestionText = response.question ?? string.Empty,
                            Type = response.type,
                            QuestionKey = response.questionKey,
                            AnswerKey = response.answerKey
                        })
                        : new List<SurveyResponseDetail>
                        {
                            new SurveyResponseDetail
                            {
                                SurveyResponseId = surveyResponse.SurveyResponseId,
                                OptionText = response.text?.Trim(),
                                QuestionText = response.question ?? string.Empty,
                                Type = response.type,
                                QuestionKey = response.questionKey,
                                AnswerKey = response.answerKey
                            }
                        }
                    ).ToList() ?? new List<SurveyResponseDetail>();

                if (surveyResponseDetails.Any())
                {
                    _dbContext.SurveyResponseDetails.AddRange(surveyResponseDetails);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation("FlowResponseAsync - added response in SurveyResponseDetail table with data = {data}", JsonConvert.SerializeObject(surveyResponseDetails));
                }

                //Message received log entry, Hussain will provide procedure and Burhan has to share json
                var flowResponseJson = JsonConvert.SerializeObject(surveyResponseDetails.Select(detail => new
                {
                    detail.OptionText,
                    detail.QuestionText,
                    detail.QuestionKey,
                    detail.AnswerKey
                })
                );

                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_MessageReceivedLogsFlow_ops @ClientId={flow.ClientId}, @SenderId={flow.SenderId}, @WaId={flowResponse.wam_Id}, @ContextWaId={flowResponse.context?.wam_Id},@Name={flowResponse.contact.name}, @PhoneNumber={flowResponse.from}, @FlowResponseJson={flowResponseJson}, @FlowToken={flowResponse.flowResponse.flowToken}").ToListAsync();
                _logger.LogInformation("Calling procedure usp_MessageReceivedLogsFlow_ops with ProcResponseTime={ProcResponseTime} ", DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

                //Mediator service
                if (response != null & response.Any())
                {
                    //_logger.LogInformation("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                    //var action = response[0];
                    //if (action.ActionType > 0 && action.ActionId > 0)
                    //{
                    //    if (action.ActionType == (int)ActionTypeEnum.TEMPLATE) //Send template or interactive message or normal message 
                    //        await _communicationService.SendInteractiveMessageAsync(action, flow.ClientId.Value, flow.SenderId.Value, flowResponse.from, flowToken: flowResponse.flowResponse.flowToken);
                    //}

                    //if (action.ModuleId == (int)ModuleEnum.Chat && action.ConversationMessageId > 0 && action.IsFoul == 0) //If conversation is going on and no foul word received
                    //{
                    //    var conversation = await _conversationService.GetConversationMessageByMessageIdAsync(clientId: flow.ClientId.Value, conversationMessageId: action.ConversationMessageId.Value, status: (int)ConversationStatusEnum.AgentAssigned);
                    //    if (conversation != null && conversation.AgentId > 0) //Check if agent id exist
                    //    {
                    //        // Look up the connection ID for the Agent ID and send the conversation
                    //        string connectionId = String.Empty;
                    //        int i;
                    //        for (i = 1; i <= 5; i++)
                    //        {
                    //            if (ConversationHub.connections.TryGetValue(conversation.AgentId ?? 0, out connectionId))
                    //            {
                    //                await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.MessageReceived.ToString(), conversation);
                    //                if (await _agentsService.IsAgentOneSignalEnabled(conversation.ClientId, conversation.SenderId))
                    //                    await _oneSignalService.SendMessageReceivedNotification(conversation.AgentId ?? 0, conversation.Language, conversation.MessageContent);
                    //                _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                    //                break;
                    //            }
                    //            else
                    //                _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                    //        }
                    //    }
                    //}

                    _logger.LogInformation("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                    var action = response[0];
                    var result = await _mediatorService.ProcessDBResponse(flow.ClientId ?? 0, flow.SenderId ?? 0, action);
                    return result;
                }

                var response1 = response != null && response.Any() ? response[0] : null;

                _logger.LogInformation("FlowResponseAsync - recieved response from db with data = {data}", JsonConvert.SerializeObject(response1));

                return new ApiResult { StatusCode = 200, Success = true, Message = "Survey response recorded successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResult { StatusCode = 0, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResult> ProcessDBResponse(int clientId, int senderId, DBResponse action)
        {
            return await _mediatorService.ProcessDBResponse(clientId, senderId, action);
        }
    }
}