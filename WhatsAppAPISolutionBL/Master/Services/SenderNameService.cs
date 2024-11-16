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
    public class SenderNameService : ISenderNameService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public SenderNameService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<USenderName>> GetSenderNameListAsync(int client_Id)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, client_Id);
            var response = await _dbContext2.SenderNames.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddSenderNameAsync(SenderNameDto senderName)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @Client_Id={1}, @Sender_Name='{2}', @Phone_Number='{3}', @Phone_Id='{4}', @App_Id='{5}', @Limit={6}, @Quality={7}, @Action_By={8}", (int)CrudEnum.Add, senderName.Client_Id, senderName.Sender_Name, senderName.Phone_Number, senderName.Phone_Id, senderName.App_Id, senderName.Limit, senderName.Quality, senderName.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateSenderNameAsync(SenderNameDto senderName)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @Sender_Id={1}, @Client_Id={2}, @Sender_Name='{3}', @Phone_Number='{4}', @Phone_Id='{5}', @App_Id='{6}', @Limit={7}, @Quality={8}, @Action_By={9}", (int)CrudEnum.Update, senderName.Sender_Id, senderName.Client_Id, senderName.Sender_Name, senderName.Phone_Number, senderName.Phone_Id, senderName.App_Id, senderName.Limit, senderName.Quality, senderName.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteSenderNameAsync(int senderName_Id)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @Sender_Id={1}", (int)CrudEnum.Delete, senderName_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
