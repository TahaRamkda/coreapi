using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Template;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateService
    {
        Task<List<UTemplate>> GetTemplateListAsync(int clientId,int senderId, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue, LanguageTypeEnum lang = LanguageTypeEnum.none, CategoryTypeEnum category = CategoryTypeEnum.none);
        Task<UResponseWithID> AddTemplateAsync(int clientId, int userId, TemplateDto template);
        Task<UResponseWithID> DeleteTemplateAsync(int templates_Id);
        Task<UTemplateDetail> GetTemplateDetailAsync(int clientId, int templateId);
        Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateStatusUpdateDto model);
        Task<List<UEntityDto>> GetTemplatesAsync(int clientId, int senderId = 0, string searchStr = "");
        Task<List<UEntity2Dto>> GetTemplateCategoriesAsync(string searchStr = "");
        Task<List<UEntity2Dto>> GetLanguagesAsync(string searchStr = "");
    }
}
