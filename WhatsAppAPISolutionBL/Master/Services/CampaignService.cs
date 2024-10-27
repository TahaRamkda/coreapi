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
    public class CampaignService : ICampaignService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public CampaignService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UClient>> GetCampaignListAsync()
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Clients.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddCampaignAsync(SendCampaignDto client)
        {
            var query = "";// string.Format(@"exec usp_Clients_Ops @ActionId={0}, @Client_Name='{1}', @Client_Language={2}, @Client_Address='{3}', @Balance={4}, @Contact_Person='{5}', @Contact_Person_Email='{6}', @Contact_Person_Phone='{7}', @Balance_Alert_Limit={8}, @Access_Token='{9}', @Action_By={10}", (int)CrudEnum.Add, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateCampaignAsync(SendCampaignDto client)
        {
            var query = "";// string.Format(@"exec usp_Clients_Ops @ActionId={0}, @Client_Id={1}, @Client_Name='{2}', @Client_Language={3}, @Client_Address='{4}', @Balance={5}, @Contact_Person='{6}', @Contact_Person_Email='{7}', @Contact_Person_Phone='{8}', @Balance_Alert_Limit={9}, @Access_Token='{10}', @Action_By={11}", (int)CrudEnum.Update, client.Client_Id, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteCampaignAsync(int campaign_Id)
        {
            var query = string.Format(@"exec usp_Clients_Ops @ActionId={0}, @Client_Id={1}", (int)CrudEnum.Delete, campaign_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<UResponse> SendCampaignAsync(SendCampaignDto sendCampaign)
        {
            var res = new UResponse();
            return res;
        }
    }
}
