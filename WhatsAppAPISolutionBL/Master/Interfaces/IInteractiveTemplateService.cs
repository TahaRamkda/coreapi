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
    }
}
