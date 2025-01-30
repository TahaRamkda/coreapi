using WhatsAppAPISolutionDL.Dto.Conversation;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IConversationService
    {
        Task<List<UConversation>> GetConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, string conversationId = "",
           string waId = "", int moduleId = 0, int parentId = 0,
           int agentId = 0, int status = 0, string phoneNumber = "",
           string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);

        Task<List<UAgentConversationList>> GetAgentConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<List<UConversationListByConversation>> GetConversationListByConversationAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int messageId = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponse> AddConversationToQueueAsync(int clientId = 0, int id = 0, string comment = "");
        Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int oldAgentId = 0, int agentId = 0, string comment = "");
        Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> model);
        //Task<ULatestConversationByConversation> GetLatestConversationMessageByConversationAsync(int clientId = 0, int id = 0);
        Task<ULatestConversationByConversation> GetConversationMessageByMessageIdAsync(int clientId = 0, int senderId = 0, int conversationId = 0, int conversationMessageId = 0, int status = 0);
        Task<List<UConversationReportList>> GetConversationReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "");
        Task<List<UConversationReportList>> GetConversationDetailReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", DateTime? fromDate = null, DateTime? toDate = null);
        Task<UResponse> ExpiredConversationNotifyToAgentAsync(List<ExpiredConversationDto> models);
    }
}
