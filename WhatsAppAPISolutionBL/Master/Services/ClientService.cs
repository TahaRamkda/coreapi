using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Client;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ClientService : IClientService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICacheService _cacheService;

        public ClientService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _cacheService = cacheService;
        }

        public async Task<List<UClient>> GetClientListAsync(string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Clients.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.List}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddClientAsync(int userId, ClientDto client)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Add}, @ClientName={client.ClientName}, @ClientLanguage={client.ClientLanguage}, @ClientAddress={client.ClientAddress}, @Balance={client.Balance}, @ContactPerson={client.ContactPerson}, @ContactPersonEmail={client.ContactPersonEmail}, @ContactPersonPhone={client.ContactPersonPhone}, @BalanceAlertLimit={client.BalanceAlertLimit}, @AccessToken={client.AccessToken}, @ActionBy={userId}, @Currency={client.Currency}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CLIENT_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> UpdateClientAsync(int userId, ClientDto client)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={client.ClientId}, @ClientName={client.ClientName}, @ClientLanguage={client.ClientLanguage}, @ClientAddress={client.ClientAddress}, @Balance={client.Balance}, @ContactPerson={client.ContactPerson}, @ContactPersonEmail={client.ContactPersonEmail}, @ContactPersonPhone={client.ContactPersonPhone}, @BalanceAlertLimit={client.BalanceAlertLimit}, @AccessToken={client.AccessToken}, @ActionBy={userId}, @Currency={client.Currency}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CLIENT_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponse> DeleteClientAsync(int ClientId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Delete}, @ClientId={ClientId}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.CLIENT_PATTERN_KEY);
            return response[0];
        }

        public async Task<List<UEntityDto>> GetClientsAsync(int clientId, string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.CLIENT_DROPDOWN_KEY, clientId, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });

            if (cacheResult == null || cacheResult.Count == 0)
                return null;

            return cacheResult;
        }

        public async Task<UClientDetail> GetClientByIdAsync(int clientId)
        {
            var cacheKey = string.Format(CacheKeys.CLIENT_BY_ID_KEY, clientId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.ClientDetails.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response[0];
            });

            if (cacheResult == null)
                return null;

            return cacheResult;
        }
    }
}
