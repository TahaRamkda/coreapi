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
                List<string> connectionList = null;
                // Retry logic for finding agent's SignalR connections
                for (int i = 0; i < 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out connectionList) && connectionList?.Any() == true)
                    {
                        break; // Exit loop if connections found
                    }
                    _logger.LogWarning("Retrying to find SignalR connections for AgentId:{AgentId}, attempt:{Attempt}", agentId, i + 1);
                }

                if (connectionList?.Any() == true)
                {
                    // Call one signal and WitAI if enabled
                    if (await _oneSignalService.IsAgentOneSignalEnabled(clientId, senderId))
                    {
                        if (!string.IsNullOrWhiteSpace(conversation.MessageContent))
                        {
                            var result = await _witAIService.SendContentToWitAi(conversation.MessageContent);
                            conversation.MessageContent = string.Concat(conversation.MessageContent, result);
                            await _oneSignalService.SendMessageReceivedNotification(agentId, conversation.Language, conversation.MessageContent);
                        }
                    }

                    // Send SignalR notification to all active connections
                    foreach (var connectionId in connectionList)
                    {
                        await _conversationHubContext.Clients.Client(connectionId).SendAsync(signalRType, conversation);
                        _logger.LogInformation("SignalR: Sent {event} to AgentId:{AgentId}, ConnectionId:{ConnectionId}, MessageId:{MessageId}", signalRType, agentId, connectionId, conversation.Id);
                    }
                }
                else
                {
                    _logger.LogError("SignalR: No connections found for AgentId:{AgentId} after retries for event {event}", agentId, signalRType);
                }
            }
        }

        public async Task ConversationAssignedNotification(int clientId, int senderId, int agentId, int oldAgentId, int conversationId, UAgentConversationList conversation)
        {
            if (conversation != null)
            {
                List<string> connectionIds = null;
                for (int i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out connectionIds) && connectionIds != null && connectionIds.Count > 0)
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            try
                            {
                                await _conversationHubContext.Clients.Client(connectionId)
                                    .SendAsync(SignalREnum.ConversationAssigned.ToString(), conversation);

                                _logger.LogInformation(
                                    "SignalR: Triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} on try {try} with object {object} and payload {payload}",
                                    SignalREnum.ConversationAssigned.ToString(), agentId, connectionId, i, conversationId,
                                    JsonConvert.SerializeObject(conversation));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(
                                    ex, "SignalR: Error sending to ConnectionId:{ConnectionId} for AgentId:{AgentId} on try {try}",
                                    connectionId, agentId, i);
                            }
                        }
                        // Send OneSignal notification only once if connections found
                        if (await _oneSignalService.IsAgentOneSignalEnabled(conversation.ClientId ?? 0, conversation.SenderId ?? 0))
                            await _oneSignalService.SendConversationAssignedNotification(conversation.AgentId ?? 0, conversation.Language, conversation.LastMessageText);

                        break; // success, no need to retry
                    }
                    else
                    {
                        _logger.LogWarning("SignalR: No connection(s) found for AgentId:{AgentId} on try {try} for event {event}",
                            agentId, i, SignalREnum.ConversationAssigned.ToString());
                    }
                }
            }

            // Send conversation unassigned
            if (oldAgentId > 0 && conversationId > 0)
            {
                await ConversationUnAssignedNotification(clientId, senderId, oldAgentId, conversationId);
            }
        }

        public async Task ConversationUnAssignedNotification(int clientId , int senderId, int agentId, int conversationId)
        {
            // Send conversation unassigned
            if (agentId > 0 && conversationId > 0)
            {
                List<string> unassignedConnectionIds = null;
                for (int i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out unassignedConnectionIds) && unassignedConnectionIds != null && unassignedConnectionIds.Any())
                    {
                        foreach (var connectionId in unassignedConnectionIds)
                        {
                            try
                            {
                                await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversationId);

                                _logger.LogInformation("SignalR, triggered event {event} for AgentId:{AgentId} and ConnectionId:{ConnectionId} with object {object} on try {try}",
                                    SignalREnum.ConversationUnAssigned.ToString(), agentId, connectionId, conversationId, i);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "SignalR, failed to send ConversationUnAssigned to ConnectionId:{ConnectionId} for AgentId:{AgentId} on try {try}",
                                    connectionId, agentId, i);
                            }
                        }

                        // Only send OneSignal once after all SignalR messages
                        if (await _oneSignalService.IsAgentOneSignalEnabled(clientId))
                            await _oneSignalService.SendConversationUnAssignedNotification(agentId);

                        break; // Successfully sent, exit retry loop
                    }
                    else
                    {
                        _logger.LogError("SignalR, no connections found for event {event} for AgentId:{AgentId} on try {try}",
                            SignalREnum.ConversationUnAssigned.ToString(), agentId, i);
                    }
                }
            }
        }
    }
}
