using WhatsAppAPISolutionDL.Dto.AppSettings;
using WhatsAppAPISolutionDL.UserModels.AppSetting;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAppSettingsService
    {
        Task<UResponse> AddAppSettingsAsync(int ClientId, int UserId, AppSettingsDto appSettingsDto);
        Task<UResponse> UpdateAppSettingsAsync(int ClientId, int UserId, AppSettingsDto appSettingsDto);
        Task<UResponse> DeleteAppSettingsAsync(int id);
        Task<List<UAppSettingsList>> GetAppSettingsAsync(int ClientId, string SearchStr = "",  int PageNo = 0, int PageSize = int.MaxValue, int senderId=0);
        Task<List<UEntityDto>> GetAllAppSettingsAsync(int clientId, string searchStr = "");
        Task<UAppSetting> GetAppSettingsByIdAsync(int id, int clientId);
        Task<UAppSetting> GetAppSettingByKeyAsync(int clientId, int senderId, string keyName);
    }
}