using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly ILogger<SignalRService> _logger;
        private readonly IHubContext<ConversationHub> _conversationHubContext;
        private readonly IOneSignalService _oneSignalService;
        private readonly IWitAIService _witAIService;

        public SignalRService(IHubContext<ConversationHub> conversationHubContext,
            ILogger<SignalRService> logger,
            IOneSignalService oneSignalService,
            IWitAIService witAIService)
        {
            _conversationHubContext = conversationHubContext;
            _logger = logger;
            _oneSignalService = oneSignalService;
            _witAIService = witAIService;
        }

        public async Task MessageReceivedNotification(int clientId, int senderId, int agentId, ULatestConversationByConversation conversation)
        {
            if (conversation != null)
            {
                string signalRType = SignalREnum.MessageReceived.ToString();

                // Look up the connection ID for the Agent ID and send the conversation
                string connectionId = String.Empty;
                int i;
                for (i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out connectionId))
                    {
                        //Call one signal and call WitAI
                        if (await _oneSignalService.IsAgentOneSignalEnabled(clientId, senderId))
                        {
                            if (!String.IsNullOrWhiteSpace(conversation.MessageContent))
                            {
                                var result = await _witAIService.SendContentToWitAi(conversation.MessageContent);
                                conversation.MessageContent = String.Concat(conversation.MessageContent, result);
                                await _oneSignalService.SendMessageReceivedNotification(agentId, conversation.Language, conversation.MessageContent);
                            }
                        }

                        //Send signalR
                        await _conversationHubContext.Clients.Client(connectionId).SendAsync(signalRType, conversation);

                        _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", signalRType, agentId, connectionId, conversation.Id, i, JsonConvert.SerializeObject(conversation));
                        break;
                    }
                    else
                        _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", signalRType, agentId, connectionId, conversation.Id, i, JsonConvert.SerializeObject(conversation));
                }
            }
        }

        public async Task ConversationAssignedNotification(int clientId, int senderId, int agentId, int oldAgentId, int conversationId, UAgentConversationList conversation)
        {
            int i;
            if (conversation != null)
            {
                // Look up the connection ID for the Agent ID and send the conversation
                string connectionId = String.Empty;
                for (i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out connectionId))
                    {
                        await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.ConversationAssigned.ToString(), conversation);

                        if (await _oneSignalService.IsAgentOneSignalEnabled(conversation.ClientId ?? 0, conversation.SenderId ?? 0))
                            await _oneSignalService.SendConversationAssignedNotification(conversation.AgentId ?? 0, conversation.Language, conversation.LastMessageText);

                        _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try} and payload {payload}", SignalREnum.ConversationAssigned.ToString(), agentId, connectionId, conversationId, i, JsonConvert.SerializeObject(conversation));
                        break;
                    }
                    else
                        _logger.LogError("SignalR, No connection found for event {event} for AgentId:{AgentId} with object {object} on try {try}", SignalREnum.ConversationAssigned.ToString(), agentId, conversationId, i);
                }
            }

            //Send conversation unassigned
            if (oldAgentId > 0 && conversationId > 0)
                await ConversationUnAssignedNotification(clientId, senderId, oldAgentId, conversationId);
        }

        public async Task ConversationUnAssignedNotification(int clientId, int senderId, int agentId, int conversationId)
        {
            //Send conversation unassigned
            if (agentId > 0 && conversationId > 0)
            {
                string unassignedConnectionId = String.Empty;
                for (int i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out unassignedConnectionId))
                    {
                        await _conversationHubContext.Clients.Client(unassignedConnectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversationId);

                        if (await _oneSignalService.IsAgentOneSignalEnabled(clientId))
                            await _oneSignalService.SendConversationUnAssignedNotification(agentId);

                        _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} with object {object} on try {try}", SignalREnum.ConversationUnAssigned.ToString(), agentId, conversationId, i);
                    }
                }
            }
        }
    }
}