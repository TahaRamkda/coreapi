using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.AppSettings;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.AppSetting;
using Microsoft.Extensions.Logging;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<AppSettingsService> _logger;
        private readonly ICacheService _cacheService;

        public AppSettingsService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2, ILogger<AppSettingsService> _logger, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            this._logger = _logger;
            _cacheService = cacheService;
        }

        public async Task<List<UAppSettingsList>> GetAppSettingsAsync(int ClientId,  string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue,int senderId = 0)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.AppSettingsList.FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @SearchStr={searchStr}, @PageNo={pageNo}, @PageSize={pageSize}, @senderId={senderId}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_AppSettings_Ops with ActionId = {actionId}, ActionName = {actionName}, ClientId = {clientId}, SearchStr = {searchStr}, PageNo = {pageNo}, PageSize = {pageSize}, ProcResponseTime = {ProcResponseTime} ms",
     (int)CrudEnum.List, CrudEnum.List, ClientId, searchStr, pageNo, pageSize, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            return response;
        }

        public async Task<UResponse> AddAppSettingsAsync(int clientId, int userId, AppSettingsDto appSettingsDto)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response
                .FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @KeyName={appSettingsDto.KeyName}, @Val={appSettingsDto.Val}, @ActionBy={userId}")
                .ToListAsync();
            
            _logger.LogInformation("Calling procedure usp_AppSettings_Ops with ActionId = {actionId}, ActionName = {actionName}, ClientId = {clientId}, KeyName = {keyName}, Val = {val}, ActionBy = {userId}, ProcResponseTime = {ProcResponseTime} ms",
           (int)CrudEnum.Add, CrudEnum.Add, clientId, appSettingsDto.KeyName, appSettingsDto.Val, userId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            await _cacheService.RemoveByPrefix(CacheKeys.APPSETTINGS_PATTERN_KEY);

            return response[0];
        }

        public async Task<UResponse> UpdateAppSettingsAsync(int clientId, int userId, AppSettingsDto appSettingsDto)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response
                .FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={clientId}, @id={appSettingsDto.Id}, @KeyName={appSettingsDto.KeyName}, @Val={appSettingsDto.Val}, @ActionBy={userId}")
                .ToListAsync();
            _logger.LogInformation("Calling procedure usp_AppSettings_Ops | ActionId = {actionId}, ActionName = {actionName}, ClientId = {clientId}, Id = {id}, KeyName = {keyName}, Val = {val}, ActionBy = {userId}, ProcResponseTime = {ProcResponseTime} ms",
    (int)CrudEnum.Update, CrudEnum.Update, clientId, appSettingsDto.Id, appSettingsDto.KeyName, appSettingsDto.Val, userId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            await _cacheService.RemoveByPrefix(CacheKeys.APPSETTINGS_PATTERN_KEY);

            return response.FirstOrDefault();
        }

        public async Task<UResponse> DeleteAppSettingsAsync(int id)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response
                .FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.Delete}, @id={id}")
                .ToListAsync();
            _logger.LogInformation("Calling procedure usp_AppSettings_Ops | ActionId = {actionId}, ActionName = {actionName}, Id = {id}, ProcResponseTime = {ProcResponseTime} ms",
            (int)CrudEnum.Delete, CrudEnum.Delete, id, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            await _cacheService.RemoveByPrefix(CacheKeys.APPSETTINGS_PATTERN_KEY);

            return response.FirstOrDefault();
        }

        public async Task<List<UEntityDto>> GetAllAppSettingsAsync(int clientId, string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.APPSETTINGS_DROPDOWN_KEY, clientId, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.Entity
                    .FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId}, @SearchStr={searchStr}")
                    .ToListAsync();
                _logger.LogInformation("Calling procedure usp_AppSettings_Ops | ActionId = {actionId}, ActionName = {actionName}, ClientId = {clientId}, SearchStr = {searchStr}, ProcResponseTime = {ProcResponseTime} ms",
                (int)CrudEnum.GetEntities, CrudEnum.GetEntities, clientId, searchStr, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                return response ?? new List<UEntityDto>();
            });
            if(cacheResult == null || cacheResult.Count == 0)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }

        public async Task<UAppSetting> GetAppSettingsByIdAsync(int id, int clientId)
        {
            var cacheKey = string.Format(CacheKeys.APPSETTINGS_BY_ID_KEY, clientId, id);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.AppSetting
                    .FromSqlInterpolated($"exec usp_AppSettings_Ops @ActionId={(int)CrudEnum.GetById}, @id={id}, @clientId={clientId}")
                    .ToListAsync();
                _logger.LogInformation("Calling procedure usp_AppSettings_Ops | ActionId = {actionId}, ActionName = {actionName}, Id = {id}, ClientId = {clientId}, ProcResponseTime = {ProcResponseTime} ms",
              (int)CrudEnum.GetById, CrudEnum.GetById, id, clientId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                return response?.FirstOrDefault();
            });
            if(cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }
    }
}
