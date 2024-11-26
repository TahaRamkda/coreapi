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
    public class CommunicationService : ICommunicationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public CommunicationService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<UResponse> AddMessageSentLogAsync(InsertMessageDto messageStatus)
        {
            var eventType = 0;
            var eventStatus = 1;
            var conversationId = "";
            var eventMessage = "";
            var pricingModel = "";
            var billable = false;
            var category = "";
            long senderId = 0;

            if (messageStatus.phone_number_Id != null)
            {
                if (!string.IsNullOrEmpty(messageStatus.phone_number_Id.display_phone_number) && !string.IsNullOrEmpty(messageStatus.phone_number_Id.phone_number_id))
                {
                    var senderName = await _dbContext.SenderNames.Where(x => x.ClientId == messageStatus.client_Id && x.PhoneNumberId == messageStatus.phone_number_Id.phone_number_id).FirstOrDefaultAsync();
                    if (senderName != null)
                        senderId = senderName.SenderId;
                }
            }

            if (messageStatus.status.ToLower() == MessageStatusEnum.SENT.ToString().ToLower())
                eventType = 1;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.DELIVERED.ToString().ToLower())
                eventType = 2;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.READ.ToString().ToLower())
                eventType = 3;
            else if (messageStatus.status.ToLower() == MessageStatusEnum.FAILED.ToString().ToLower())
            {
                eventType = 4;
                eventStatus = 0;
            }

            if (messageStatus.conversation != null)
                conversationId = messageStatus.conversation.id;

            if (messageStatus.error != null)
                eventMessage = messageStatus.error.error_Details;

            if (messageStatus.pricing != null)
            {
                pricingModel = messageStatus.pricing.pricing_model;
                billable = messageStatus.pricing.billable;
                category = messageStatus.pricing.category;
            }

            var query = string.Format(@"exec usp_MessageSentLogs_StatusUpdate @ModuleId={0}, @ClientId={1}, @ParentId={2}, @SenderId={3}, @PhoneNumber='{4}', @WaId='{5}', @WaId2='{6}', @EventType={7}, @EventTime='{8}', @EventStatus={9}, @EventMessage='{10}', @PricingModel='{11}', @Billable={12}, @Category='{13}', @TemplateId={14}", messageStatus.module_Id, messageStatus.client_Id, messageStatus.parent_Id, senderId, messageStatus.recipient_Id, messageStatus.wam_Id, conversationId, eventType, messageStatus.update_dateTime, eventStatus, eventMessage, pricingModel, billable, category, messageStatus.template_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage)
        {
            return new UResponse();
        }
    }
}
