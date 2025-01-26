using WhatsAppAPISolutionDL.Dto.SystemActions;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.SystemActions;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISystemActionsService
    {
        Task<List<USystemActions>> GetSystemActionsListAsync(int ClientId, int SystemActionId = 0, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponse> AddSystemActionsAsync(SystemActionsDto systemActions);
        Task<UResponse> UpdateSystemActionsAsync(SystemActionsDto systemActions);
        Task<UResponse> DeleteSystemActionsAsync(int systemActionsId);
        Task<List<UEntityDto>> GetSystemActionsAsync(int clientId, string searchStr = "");
        Task<USystemActionsDetail> GetSystemActionsByIdAsync(int clientId, int id);
    }
}
