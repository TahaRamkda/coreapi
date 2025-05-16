using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAgentsService
    {
        Task<List<UAgent>> GetAgentListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponse> AddAgentAsync(int clientId, int userId, AgentDto agent);
        Task<UResponse> UpdateAgentAsync(int clientId, int userId, AgentDto agent);
        Task<UResponse> DeleteAgentAsync(int AgentId);
        Task<UResponse> SetAgentStatusAsync(int agentId, int status);
        Task<UResponse> SetAgentDisableAsync(int clientId, int agentId, bool disable);
        Task<UResponse> AddAgentTimingsAsync(int clientId, int userId, AgentTimingDto model);
        Task<List<UAgentTiming>> GetAgentTimingListAsync(int clientId, int agentId);
        Task<List<UEntityDto>> GetAgentsAsync(int clientId, int senderId = 0, string searchStr = "");
        Task<List<UEntityDto>> GetActiveAgentsAsync(int clientId, int senderId = 0, string searchStr = "");
        Task<UAgentDetail> GetAgentByIdAsync(int clientId, int agentId);
        Task<List<UAgentSupervisorReport>> GetAgentSupervisorReportListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<List<UAgentSupervisorReport>> GetAgentDetailSupervisorReportListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UAgentStat> GetAgentStatsAsync(int clientId, int agentId, int senderId = 0);
        Task<UResponse> ImportBulkAgentTimings(int clientId, int userId, ImportBulkAgentTimingDto model);
    }
}
