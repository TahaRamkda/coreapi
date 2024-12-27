using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAgentsService
    {
        Task<List<UAgent>> GetAgentListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponse> AddAgentAsync(AgentDto agent);
        Task<UResponse> UpdateAgentAsync(AgentDto agent);
        Task<UResponse> DeleteAgentAsync(int AgentId);
        Task<UResponse> SetAgentStatusAsync(int id, int status);
        Task<UResponse> AddAgentTimingsAsync(AgentTimingDto model);
        Task<List<UAgentTiming>> GetAgentTimingListAsync(int clientId, int agentId);
        Task<List<UEntityDto>> GetAgentsAsync(int clientId, int senderId = 0, string searchStr = "");
        Task<UGetAgentById> GetAgentByIdAsync(int clientId, int agentId);
    }
}
