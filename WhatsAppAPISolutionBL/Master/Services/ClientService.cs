using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ClientService : IClientService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ClientService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UClient>> GetClientListAsync(string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Clients.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.List}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddClientAsync(ClientDto client)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Add}, @ClientName={client.ClientName}, @ClientLanguage={client.ClientLanguage}, @ClientAddress={client.ClientAddress}, @Balance={client.Balance}, @ContactPerson={client.ContactPerson}, @ContactPersonEmail={client.ContactPersonEmail}, @ContactPersonPhone={client.ContactPersonPhone}, @BalanceAlertLimit={client.BalanceAlertLimit}, @AccessToken={client.AccessToken}, @ActionBy={client.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateClientAsync(ClientDto client)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={client.ClientId}, @ClientName={client.ClientName}, @ClientLanguage={client.ClientLanguage}, @ClientAddress={client.ClientAddress}, @Balance={client.Balance}, @ContactPerson={client.ContactPerson}, @ContactPersonEmail={client.ContactPersonEmail}, @ContactPersonPhone={client.ContactPersonPhone}, @BalanceAlertLimit={client.BalanceAlertLimit}, @AccessToken={client.AccessToken}, @ActionBy={client.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteClientAsync(int ClientId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.Delete}, @ClientId={ClientId}").ToListAsync();
            return response[0];
        }

        public async Task<List<UEntityDto>> GetClientsAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<UClientDetail> GetClientByIdAsync(int clientId)
        {
            var response = await _dbContext2.ClientDetails.FromSqlInterpolated($"exec usp_Clients_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
