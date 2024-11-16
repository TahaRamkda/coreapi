using Microsoft.AspNetCore.Http;
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
    public class CustomIntegrationService : ICustomIntegrationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public CustomIntegrationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<UResponse> SendSmsAsync(SendSmsDto sendSms, int ClientId)
        {
            sendSms.BrandName = sendSms.BrandName.Replace(" ", "");
            var templateName = string.Concat(sendSms.BrandName, "_", sendSms.TemplateName);
            var templateDetails = await _dbContext.Templates.Where(x => x.TemplateName == templateName && x.ClientId == ClientId).FirstOrDefaultAsync();


            var query = "";// string.Format(@"exec usp_Clients_Ops @ActionId={0}, @Client_Name='{1}', @Client_Language={2}, @Client_Address='{3}', @Balance={4}, @Contact_Person='{5}', @Contact_Person_Email='{6}', @Contact_Person_Phone='{7}', @Balance_Alert_Limit={8}, @Access_Token='{9}', @Action_By={10}", (int)CrudEnum.Add, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
