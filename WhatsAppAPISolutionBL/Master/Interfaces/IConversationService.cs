using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

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

        Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int agentId = 0, string comment = "");
        Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> model);
        Task<UConversationListByConversation> GetLatestConversationMessageByConversationAsync(int clientId = 0, int id = 0);
    }
}
