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
    public class SenderNameService : ISenderNameService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public SenderNameService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<USenderName>> GetSenderNameListAsync(int ClientId)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, ClientId);
            var response = await _dbContext2.SenderNames.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddSenderNameAsync(SenderNameDto senderName)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @ClientId={1}, @SenderName='{2}', @PhoneNumber='{3}', @PhoneId='{4}', @AppId='{5}', @Limit={6}, @Quality={7}, @ActionBy={8}", (int)CrudEnum.Add, senderName.ClientId, senderName.SenderName, senderName.PhoneNumber, senderName.PhoneId, senderName.AppId, senderName.Limit, senderName.Quality, senderName.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateSenderNameAsync(SenderNameDto senderName)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @SenderId={1}, @ClientId={2}, @SenderName='{3}', @PhoneNumber='{4}', @PhoneId='{5}', @AppId='{6}', @Limit={7}, @Quality={8}, @ActionBy={9}", (int)CrudEnum.Update, senderName.SenderId, senderName.ClientId, senderName.SenderName, senderName.PhoneNumber, senderName.PhoneId, senderName.AppId, senderName.Limit, senderName.Quality, senderName.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteSenderNameAsync(int SenderNameId)
        {
            var query = string.Format(@"exec usp_SenderNames_Ops @ActionId={0}, @SenderId={1}", (int)CrudEnum.Delete, SenderNameId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
