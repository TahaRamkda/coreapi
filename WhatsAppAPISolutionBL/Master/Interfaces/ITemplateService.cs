using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateService
    {
        Task<List<UTemplate>> GetTemplateListAsync(int clientId, int transactionType = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
        Task<UResponseWithID> AddTemplateAsync(TemplateDto template);
        Task<UResponseWithID> UpdateTemplateAsync(TemplateDto template);
        Task<UResponseWithID> DeleteTemplateAsync(int templates_Id);
        Task<UTemplateDetails> GetTemplateDetailsAsync(int client_Id, int template_Id = 0);
        Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateDto template);
        Task<List<UEntityDto>> GetTemplatesAsync(int clientId, int defaultType = 0, int senderId = 0, int transactionType = 0, string searchStr = "");
        Task<List<UEntity2Dto>> GetTemplateCategoriesAsync(string searchStr = "");
        Task<List<UEntity2Dto>> GetLanguagesAsync(string searchStr = "");
        Task<List<UDefaultTemplateList>> GetDefaultTemplateListAsync(int clientId, int senderId = 0, int templateId = 0, int defaultType = 0, string searchStr = "");
    }
}
