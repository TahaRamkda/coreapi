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
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.Message;

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

        public MessageService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IMediaService mediaService,
            ICommunicationService communicationService,
            ILogger<MessageService> logger,
            IHubContext<ConversationHub> conversationHubContext,
            IConversationService conversationService,
            IOneSignalService oneSignalService,
            IAgentsService agentsService)
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
        }

        #region Utilities

        private async Task<bool> IsFoulMessage(int clientId, int? senderId = 0, string msg = null)
        {
            if (String.IsNullOrWhiteSpace(msg))
                return false;

            string keyNames = CommonEnum.FoulLanguageWords.ToString();
            var response = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={senderId}").ToListAsync();

            // Check if the response contains data
            if (response == null || !response.Any())
                return false;

            // Extract foul words from the first setting
            var foulLanguageSetting = response.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(foulLanguageSetting?.Val))
                return false;

            // Parse the foul words and clean up any whitespace
            var foulWords = foulLanguageSetting.Val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(word => word.Trim()).Where(word => !string.IsNullOrWhiteSpace(word)).ToArray();

            // Check for whole-word matches in the message using LINQ and Regex
            return foulWords.Any(word => System.Text.RegularExpressions.Regex.IsMatch(msg, $@"\b{System.Text.RegularExpressions.Regex.Escape(word)}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        #endregion

        public async Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus)
        {
            var messageStatusEnum = (MessageStatusEnum)Enum.Parse(typeof(MessageStatusEnum), messageStatus.status.ToUpper());

            var eventType = (int)messageStatusEnum;
            var eventStatus = messageStatusEnum == MessageStatusEnum.FAILED ? 0 : 1;
            var conversationId = "";
            var eventMessage = "";
            var pricingModel = "";
            var billable = false;
            var category = "";
            int senderId = 0;

            if (messageStatus.phone_number_Id != null
                && !String.IsNullOrEmpty(messageStatus.phone_number_Id.display_phone_number)
                && !String.IsNullOrEmpty(messageStatus.phone_number_Id.phone_number_id))
            {
                var senderName = await _dbContext.SenderNames.Where(x => x.ClientId == Convert.ToInt32(messageStatus.client_Id) && x.PhoneNumberId == messageStatus.phone_number_Id.phone_number_id).FirstOrDefaultAsync();
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
            }

            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={0}, @ClientId={messageStatus.client_Id}, @ParentId={0}, @SenderId={senderId}, @PhoneNumber={messageStatus.recipient_Id}, @WaId={messageStatus.wam_Id}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={messageStatus.update_dateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}").ToListAsync();
            _logger.LogDebug("Calling procedure usp_MessageSentLogs_StatusUpdate with ProcResponseTime={ProcResponseTime} ", DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response[0];
        }

        /// <summary>
        /// Add message received logs
        /// </summary>
        /// <param name="messageReceive"></param>
        /// <returns></returns>
        public async Task<UMessageReceived> AddMessageReceivedLogAsync(WhatsAppMessageReceiveDto messageReceive)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == Convert.ToInt32(messageReceive.client_Id));
            var senderName = await _dbContext.SenderNames.FirstOrDefaultAsync(x => x.PhoneNumberId == messageReceive.phone_number_Id.phone_number_id);

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
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.UMessageReceiveds.FromSqlInterpolated($"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id},@Name={fullName}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}, @IsFoul ={isFoulMsg}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_MessageReceivedLogs_ops with ProcResponseTime={ProcResponseTime} ", DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            //Central service call
            if (response != null & response.Any())
            {
                _logger.LogInformation("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                var action = response[0];
                if (action.ActionType > 0 && action.ActionId > 0)
                {
                    //var flowToken = "C:"+ client.ClientId+"|S:"+ senderName.SenderId+"|M:"+ response[0].ModuleId + "|P:" + response[0].ParentId;
                    var flowToken = $"{FlowIdentifier.ClientId}:{client.ClientId}|" + $"{FlowIdentifier.SenderId}:{senderName.SenderId}|" + $"{FlowIdentifier.ModuleId}:{action.ModuleId}|" + $"{FlowIdentifier.ParentId}:{action.ParentId}";
                    if (action.ActionType == (int)ActionTypeEnum.TEMPLATE) //Send template or interactive message or normal message 
                        await _communicationService.SendInteractiveMessageAsync(action, client.ClientId, senderName.SenderId, messageReceive.from, flowToken: flowToken);
                }

                if (action.ModuleId == (int)ModuleEnum.Chat && action.ConversationMessageId > 0 && action.IsFoul == 0) //If conversation is going on and no foul word received
                {
                    var conversation = await _conversationService.GetConversationMessageByMessageIdAsync(clientId: client.ClientId, conversationMessageId: action.ConversationMessageId.Value, status: (int)ConversationStatusEnum.AgentAssigned);
                    if (conversation != null && conversation.AgentId > 0) //Check if agent id exist
                    {


                        var client1 = new HttpClient();
                        var apiUrl = $"https://api.wit.ai/message?v=20250405&q={conversation.MessageContent}";
                        client1.DefaultRequestHeaders.Clear();
                        client1.DefaultRequestHeaders.Add("Authorization", "Bearer XKNTI6I446CSPCAJ52VABGCSVPYS2QCI");

                        var response1 = await client1.GetAsync(apiUrl);
                        _logger.LogInformation("FlowResponseAsync - getting response from wit.ai response = {response}", JsonConvert.SerializeObject(response1));
                        string resultString = string.Empty;
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
                                    resultString += $" - Sentiment: {sentiment.Value} ({sentiment.Confidence:F3})";
                                }
                            }
                        }
                        conversation.MessageContent += resultString;
                        _logger.LogInformation("FlowResponseAsync - merging wit.ai response message with conversation MessageContent = {msg}", conversation.MessageContent);

                        // Look up the connection ID for the Agent ID and send the conversation
                        string connectionId = String.Empty;
                        int i;
                        for (i = 1; i <= 5; i++)
                        {
                            if (ConversationHub.connections.TryGetValue(conversation.AgentId ?? 0, out connectionId))
                            {
                                await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.MessageReceived.ToString(), conversation);
                                if (await _agentsService.IsAgentOneSignalEnabled(conversation.ClientId, conversation.SenderId))
                                    await _oneSignalService.SendMessageReceivedNotification(conversation);
                                _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                                break;
                            }
                            else
                                _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                        }

                        //if (i >= 5) // If max retry exceeded, unassign the conversation again
                        //    await _conversationService.AddConversationToQueueAsync(clientId: conversation.ClientId ?? 0, id: conversation.Id ?? 0, comment: "Cannot send the conversation to agent!");

                        ////Send to all the agents except the agent that has been assigned just now
                        //if (!String.IsNullOrEmpty(connectionId))
                        //    await _conversationHubContext.Clients.AllExcept(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversation.Id ?? 0);
                        //else
                        //    await _conversationHubContext.Clients.All.SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversation.Id ?? 0);
                    }
                }
            }

            return response != null && response.Any() ? response[0] : null;
        }

        public async Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model)
        {
            return await _communicationService.SendAgentMessageAsync(model);
        }

        public async Task<ApiResult> SendAgentInteractiveMessageAsync(SendAgentInteractiveMessageRequestDto model)
        {
            return await _communicationService.SendAgentInteractiveMessageAsync(model);
        }

        public async Task<UResponse> FlowResponseAsync(FlowResponseDto flowResponse)
        {
            try
            {
                if (flowResponse == null || string.IsNullOrEmpty(flowResponse.from))
                    return new UResponse { Status = 0, Message = "Invalid request data" };

                if (flowResponse.flowResponse == null)
                    return new UResponse { Status = 0, Message = "Flow response is required" };

                if (string.IsNullOrEmpty(flowResponse.flowResponse.flowToken))
                    return new UResponse { Status = 0, Message = "Flow token is required" };

                string fullName = flowResponse.contact != null ? flowResponse.contact.name : String.Empty;
                string flowToken = flowResponse.flowResponse.flowToken;
                var flowId = Convert.ToInt32(flowToken.ParseIdPath<FlowTokenIdentifier>().path.FlowId);
                var parentId = Convert.ToInt32(flowToken.ParseIdPath<FlowTokenIdentifier>().path.ParentId);
                var moduleId = Convert.ToInt32(flowToken.ParseIdPath<FlowTokenIdentifier>().path.ModuleId);
                if (flowId == 0)
                    return new UResponse { Status = 0, Message = "Invalid FlowId" };

                // Fetch Flow using MetaFlowId (Ensure correct field is used)
                var flow = await _dbContext.Flows.FirstOrDefaultAsync(x => x.FlowId == flowId);
                if (flow == null)
                    return new UResponse { Status = 0, Message = "No flow found with this MetaFlowId" };

                //If survey, add into survey table
                if (flow.ModuleId == (int)ModuleEnum.Survey)
                {
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
                        ModuleId = moduleId,
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
                    var flowResponseJson = System.Text.Json.JsonSerializer.Serialize(surveyResponseDetails.Select(detail => new
                    {
                        detail.OptionText,
                        detail.QuestionText,
                        detail.QuestionKey,
                        detail.AnswerKey
                    })
                    );
                    var startProcTime = DateTime.UtcNow;
                    var response = await _dbContext2.UMessageReceiveds.FromSqlInterpolated($"exec usp_MessageReceivedLogsFlow_ops @ClientId={flow.ClientId}, @SenderId={flow.SenderId}, @WaId={flowResponse.wam_Id}, @ContextWaId={flowResponse.context?.wam_Id},@Name={fullName}, @PhoneNumber={flowResponse.from}, @FlowResponseJson={flowResponseJson}, @FlowToken={flowToken}").ToListAsync();
                    _logger.LogInformation("Calling procedure usp_MessageReceivedLogsFlow_ops with ProcResponseTime={ProcResponseTime} ", DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

                    //Central service call
                    if (response != null & response.Any())
                    {
                        _logger.LogInformation("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                        var action = response[0];
                        if (action.ActionType > 0 && action.ActionId > 0)
                        {
                            if (action.ActionType == (int)ActionTypeEnum.TEMPLATE) //Send template or interactive message or normal message 
                                await _communicationService.SendInteractiveMessageAsync(action, flow.ClientId.Value, flow.SenderId.Value, flowResponse.from, flowToken: flowToken);
                        }

                        if (action.ModuleId == (int)ModuleEnum.Chat && action.ConversationMessageId > 0 && action.IsFoul == 0) //If conversation is going on and no foul word received
                        {
                            var conversation = await _conversationService.GetConversationMessageByMessageIdAsync(clientId: flow.ClientId.Value, conversationMessageId: action.ConversationMessageId.Value, status: (int)ConversationStatusEnum.AgentAssigned);
                            if (conversation != null && conversation.AgentId > 0) //Check if agent id exist
                            {
                                // Look up the connection ID for the Agent ID and send the conversation
                                string connectionId = String.Empty;
                                int i;
                                for (i = 1; i <= 5; i++)
                                {
                                    if (ConversationHub.connections.TryGetValue(conversation.AgentId ?? 0, out connectionId))
                                    {
                                        await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.MessageReceived.ToString(), conversation);
                                        if (await _agentsService.IsAgentOneSignalEnabled(conversation.ClientId, conversation.SenderId))
                                            await _oneSignalService.SendMessageReceivedNotification(conversation);
                                        _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                                        break;
                                    }
                                    else
                                        _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, connectionId, conversation.Id ?? 0, i, JsonConvert.SerializeObject(conversation));
                                }
                            }
                        }
                    }

                    var response1 = response != null && response.Any() ? response[0] : null;

                    _logger.LogInformation("FlowResponseAsync - recieved response from db with data = {data}", JsonConvert.SerializeObject(response1));

                }
                return new UResponse { Status = 1, Message = "Survey response recorded successfully" };
            }
            catch (Exception ex)
            {
                return new UResponse { Status = 0, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
