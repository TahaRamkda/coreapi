using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISignalRService
    {
        Task MessageReceivedNotification(int clientId, int senderId, int agentId, ULatestConversationByConversation conversation);
        Task ConversationAssignedNotification(int clientId, int senderId, int agentId, int oldAgentId, int conversationId, UAgentConversationList conversation);
        Task ConversationUnAssignedNotification(int clientId, int senderId, int agentId, int conversationId);
        Task MessageStatusNotification(int clientId, int senderId, int agentId, int MessageId, int status, string ConversationId);

    }
}
