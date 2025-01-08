using WhatsAppAPISolutionDL.Dto.InteractiveTemplate;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.InteractiveTemplate;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IInteractiveTemplateService
    {
        Task<List<UInteractiveTemplate>> GetInteractiveTemplateListAsync(int clientId, int senderId = 0, string searchStr = "", DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponse> AddInteractiveTemplateAsync(InteractiveTemplateDto model);
        Task<UResponse> UpdateInteractiveTemplateAsync(InteractiveTemplateDto model);
        Task<UInteractiveTemplateDetail> GetInteractiveTemplateDetailsAsync(int clientId, int senderId, int interactiveTemplateId);
        Task<List<UEntityDto>> GetAgentInteractiveTemplatesAsync(int clientId, int senderId, string language = "", string searchStr = "");
        Task<List<UEntityDto>> GetInteractiveTemplateWithoutParamsAsync(int clientId, int senderId, string language = "", string searchStr = "");
    }
}
