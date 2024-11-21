using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ClientService: IClientService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ClientService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UClient>> GetClientListAsync()
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Clients.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddClientAsync(ClientDto client)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientName='{1}', @ClientLanguage={2}, @ClientAddress='{3}', @Balance={4}, @ContactPerson='{5}', @ContactPersonEmail='{6}', @ContactPersonPhone='{7}', @BalanceAlertLimit={8}, @AccessToken='{9}', @ActionBy={10}", (int)CrudEnum.Add, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateClientAsync(ClientDto client)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientId={1}, @ClientName='{2}', @ClientLanguage={3}, @ClientAddress='{4}', @Balance={5}, @ContactPerson='{6}', @ContactPersonEmail='{7}', @ContactPersonPhone='{8}', @BalanceAlertLimit={9}, @AccessToken='{10}', @ActionBy={11}", (int)CrudEnum.Update, client.Client_Id, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteClientAsync( int client_Id)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.Delete, client_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
