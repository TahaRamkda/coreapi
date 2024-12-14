using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAgentsService
    {
        Task<List<UAgent>> GetAgentListAsync(int ClientId, string SearchStr = "", int Status = 0);
        Task<UResponse> AddAgentAsync(AgentDto agent);
        Task<UResponse> UpdateAgentAsync(AgentDto agent);
        Task<UResponse> DeleteAgentAsync(int AgentId);
        Task<UResponse> SetAgentStatusAsync(int id, int status);
        Task<UResponse> AddAgentTimingsAsync(AgentTimingDto model);
        Task<List<UAgentTiming>> GetAgentTimingListAsync(int clientId, int agentId);
    }
}
