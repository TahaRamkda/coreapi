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
    public class MessageService : IMessageService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public MessageService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<UResponse> UpdateMessageStatusAsync(WhatsAppMessageStatusUpdateDto messageStatus)
        {
            var query = string.Format(@"exec usp_Campaign_UpdateSendStatus @ModuleId={0}, @Client_Id={1}, @Parent_Id={2}, @Sender_Id={3}, @PhoneNumber='{4}', @wa_id='{5}', @wa_id2='{6}', @EventType={7}, @EventTime='{8}', @Event_Status={9}, @Event_message='{10}', @PricingModel='{11}', @billable={12}, @category={13}", 1, messageStatus.client_Id, 0, 1, messageStatus.recipient_Id, messageStatus.wam_Id, "", 1, 1);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
