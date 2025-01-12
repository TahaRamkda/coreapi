using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
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

        public MessageService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IMediaService mediaService,
            ICommunicationService communicationService,
            ILogger<MessageService> logger,
            IHubContext<ConversationHub> conversationHubContext,
            IConversationService conversationService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _mediaService = mediaService;
            _communicationService = communicationService;
            _logger = logger;
            _conversationHubContext = conversationHubContext;
            _conversationService = conversationService;
        }

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

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={0}, @ClientId={messageStatus.client_Id}, @ParentId={0}, @SenderId={senderId}, @PhoneNumber={messageStatus.recipient_Id}, @WaId={messageStatus.wam_Id}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={messageStatus.update_dateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}").ToListAsync();
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

            if (messageReceive.type == MessageReceiveTypeEnum.BUTTON.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; // Convert.ToInt32(MessageReceiveTypeEnum.BUTTON);
                messageText = messageReceive.button.payload ?? "";
            }
            else if (messageReceive.type == MessageReceiveTypeEnum.TEXT.ToString())
            {
                messageType = (int)MainMessageTypeEnum.TEXT; //Convert.ToInt32(MessageReceiveTypeEnum.TEXT);
                messageText = messageReceive.text.body ?? "";
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

            var response = await _dbContext2.UMessageReceiveds.FromSqlInterpolated($"exec usp_MessageReceivedLogs_ops @ClientId={messageReceive.client_Id}, @SenderId={senderName?.SenderId}, @WaId={messageReceive.wam_Id}, @ContextWaId={messageReceive.context?.wam_Id},@Name={fullName}, @PhoneNumber={messageReceive.from}, @ResponseType={messageType}, @ResponseText={messageText}, @MediaId={mediaId}").ToListAsync();

            //Central service call
            if (response != null & response.Any())
            {
                _logger.LogInformation("Message Received Log DB call response: {response}", JsonConvert.SerializeObject(response[0]));
                var action = response[0];
                if (action.ActionType > 0 && action.ActionId > 0)
                {
                    if (action.ActionType == (int)ActionTypeEnum.TEMPLATE) //Send template or interactive message or normal message 
                        await _communicationService.SendInteractiveMessageAsync(action, client.ClientId, senderName.SenderId, messageReceive.from);
                }

                if (action.ModuleId == (int)ModuleEnum.Chat && action.ParentId > 0) //If conversation is going on
                {
                    var conversation = await _conversationService.GetLatestConversationMessageByConversationAsync(clientId: Convert.ToInt32(messageReceive.client_Id), id: action.ParentId.Value);
                    if (conversation != null)
                    {
                        // Look up the connection ID for the Agent ID and send the conversation
                        string connectionId = String.Empty;
                        int i;
                        for (i = 1; i <= 5; i++)
                        {
                            if (ConversationHub.connections.TryGetValue(conversation.AgentId ?? 0, out connectionId))
                            {
                                await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.MessageReceived.ToString(), conversation);
                                _logger.LogInformation("SignalR, triggered event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, conversation.Id ?? 0, i);
                                break;
                            }
                            else
                                _logger.LogError("SignalR, No connection found for event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.MessageReceived.ToString(), conversation.AgentId ?? 0, conversation.Id ?? 0, i);
                        }

                        if (i >= 5) // If max retry exceeded, unassign the conversation again
                            await _conversationService.AddConversationToQueueAsync(clientId: conversation.ClientId ?? 0, id: conversation.Id ?? 0, comment: "Cannot send the conversation to agent!");

                        //Send to all the agents except the agent that has been assigned just now
                        if (!String.IsNullOrEmpty(connectionId))
                            await _conversationHubContext.Clients.AllExcept(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversation.Id ?? 0);
                        else
                            await _conversationHubContext.Clients.All.SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversation.Id ?? 0);
                    }
                    else
                        _logger.LogError("Cannot find conversation with ClientId {ClientId} and Id {Id}", conversation.AgentId ?? 0, conversation.Id ?? 0);
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
    }
}
