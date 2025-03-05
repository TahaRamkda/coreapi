using WhatsAppAPISolutionDL.DTO.Survey;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Flow;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IFlowsService
    {
        Task<List<UFlow>> GetFlowListAsync(int clientId, string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponse> AddFlowAsync(int clientId, int userId, FlowDTO obj);
        Task<UResponse> UpdateFlowAsync(int clientId, int userId, FlowDTO obj);
        Task<UResponse> PublishFlowAsync(int clientId, int flowId);
        Task<UResponse> DeleteFlowAsync(int flowId);
        Task<FlowDTO> GetFlowDetailsByIdAsync(int flowId);
        Task<List<UEntityDto>> GetFlowsAsync(int clientId, int senderId = 0, string searchStr = "");
    }
}
