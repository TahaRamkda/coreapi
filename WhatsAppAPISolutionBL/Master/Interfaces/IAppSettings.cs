using Microsoft.AspNetCore.Identity.Data;
using WhatsAppAPISolutionDL.Dto.AppSettings;
using WhatsAppAPISolutionDL.UserModels.AppSetting;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Group;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAppSettingsService
    {
        Task<UResponse> AddAppSettingsAsync(int ClientId, int UserId, AppSettingsDto appSettingsDto);
        Task<UResponse> UpdateAppSettingsAsync(int ClientId, int UserId, AppSettingsDto appSettingsDto);
        Task<UResponse> DeleteAppSettingsAsync(int id);
        Task<List<UAppSetting>> GetAppSettingsAsync(int ClientId, string SearchStr = "",  int PageNo = 0, int PageSize = int.MaxValue);
        Task<List<UEntityDto>> GetAllAppSettingsAsync(int clientId, string searchStr = "");
        Task<UAppSetting> GetAppSettingsByIdAsync(int id, int clientId);
    }
}