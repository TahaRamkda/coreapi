namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOneSignalService
    { 
        Task SendConversationAssignedNotification(int agentId, string language, string message);
        Task SendMessageReceivedNotification(int agentId, string language, string message);
        Task SendConversationUnAssignedNotification(int agentId);
    }
}
