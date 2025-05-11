using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Conversation;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMediatorService
    {
        /// <summary>
        /// Centralized function to process all type of DB response
        /// </summary>
        /// <param name="dBResponse"></param>
        /// <returns></returns>
        Task<ApiResult> ProcessDBResponse(int clientId, int senderId, DBResponse model);
        Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> models);
        Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int oldAgentId = 0, int agentId = 0, string comment = "");
        Task<UResponse> ExpiredConversationNotifyToAgentAsync(List<ExpiredConversationDto> models);
    }
}
