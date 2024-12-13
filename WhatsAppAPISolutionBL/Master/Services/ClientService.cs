using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @SearchStr='{1}', @SortBy={2}, @PageNo={3}, @PageSize={4}", (int)CrudEnum.List, SearchStr, SortBy, PageNo, PageSize);
            var response = await _dbContext2.Clients.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddClientAsync(ClientDto client)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientName='{1}', @ClientLanguage={2}, @ClientAddress='{3}', @Balance={4}, @ContactPerson='{5}', @ContactPersonEmail='{6}', @ContactPersonPhone='{7}', @BalanceAlertLimit={8}, @AccessToken='{9}', @ActionBy={10}", (int)CrudEnum.Add, client.ClientName, client.ClientLanguage, client.ClientAddress, client.Balance, client.ContactPerson, client.ContactPersonEmail, client.ContactPersonPhone, client.BalanceAlertLimit, client.AccessToken, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateClientAsync(ClientDto client)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientId={1}, @ClientName='{2}', @ClientLanguage={3}, @ClientAddress='{4}', @Balance={5}, @ContactPerson='{6}', @ContactPersonEmail='{7}', @ContactPersonPhone='{8}', @BalanceAlertLimit={9}, @AccessToken='{10}', @ActionBy={11}", (int)CrudEnum.Update, client.ClientId, client.ClientName, client.ClientLanguage, client.ClientAddress, client.Balance, client.ContactPerson, client.ContactPersonEmail, client.ContactPersonPhone, client.BalanceAlertLimit, client.AccessToken, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteClientAsync(int ClientId)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.Delete, ClientId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
