using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.SenderName;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SenderNameService : ISenderNameService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICacheService _cacheService;

        public SenderNameService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _cacheService = cacheService;
        }

        public async Task<List<USenderName>> GetSenderNameListAsync(int ClientId)
        { 
            var cacheKey = string.Format(CacheKeys.SENDERNAME_DROPDOWN_KEY, ClientId);
            var cacheResult = _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.SenderNames.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
          
            return await cacheResult;
        }

        public async Task<UResponse> AddSenderNameAsync(int clientId, int userId, SenderNameDto senderName)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @SenderName={senderName.SenderName}, @PhoneNumber={senderName.PhoneNumber}, @PhoneId={senderName.PhoneId}, @AppId={senderName.AppId}, @Limit={senderName.Limit}, @Quality={senderName.Quality}, @MediaId={senderName.MediaId}, @Verified={senderName.Verified}, @ActionBy={userId}, @WebsiteUrl={senderName.WebsiteUrl}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.SENDERNAME_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> UpdateSenderNameAsync(int clientId, int userId, SenderNameDto senderName)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Update}, @SenderId={senderName.SenderId}, @ClientId={clientId}, @SenderName={senderName.SenderName}, @PhoneNumber={senderName.PhoneNumber}, @PhoneId={senderName.PhoneId}, @AppId={senderName.AppId}, @Limit={senderName.Limit}, @Quality={senderName.Quality}, @MediaId={senderName.MediaId},@Verified={senderName.Verified}, @ActionBy={userId}, @WebsiteUrl={senderName.WebsiteUrl}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.SENDERNAME_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> DeleteSenderNameAsync(int SenderNameId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.Delete}, @SenderId={SenderNameId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.SENDERNAME_PATTERN_KEY);
            return response[0];
        }

        public async Task<List<UEntityDto>> GetSenderNamesAsync(int clientId, string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.SENDERNAME_DROPDOWN_KEY2, clientId, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });

            if (cacheResult == null || cacheResult.Count == 0)
                await _cacheService.RemoveAsync(cacheKey);
            
            return cacheResult;
        }

        public async Task<USenderNameDetail> GetSenderNameByIdAsync(int clientId, int senderId)
        {
            var cacheKey = string.Format(CacheKeys.SENDERNAME_BY_ID_KEY, clientId, senderId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.SenderNameDetails.FromSqlInterpolated($"exec usp_SenderNames_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}, @SenderId={senderId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response[0];
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            
            return cacheResult;
        }
    }
}
