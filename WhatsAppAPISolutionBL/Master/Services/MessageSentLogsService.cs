using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MessageSentLogsService : IMessageSentLogsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public MessageSentLogsService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int ClientId, int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.MessageSentLogs.FromSqlInterpolated($"exec usp_MessageSentLogs_Ops @ActionId={(int)CrudEnum.List}, @Id={Id}, @ClientId={ClientId}, @ModuleId={ModuleId}, @ParentId={ParentId}, @PhoneNumber={PhoneNumber}, @WaId={WaId},@WaId2={WaId2},@SenderId={SenderId},@FromSentDate={FromSentDate}, @ToSentDate={ToSentDate}, @FromDeliveredDate={FromDeliveredDate}, @ToDeliveredDate={ToDeliveredDate}, @FromReadDate={FromReadDate}, @ToReadDate={ToReadDate},@FromDate={FromDate}, @ToDate={ToDate}, @CurrentStatus={CurrentStatus}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNumber={PageNo}, @PageSize={PageSize}").ToListAsync();

            return response;
        }

        public async Task<UResponse> AddMessageSentLogAsync(InsertMessageDto messageStatus)
        {
            int eventType = (int)messageStatus.Status;
            int eventStatus = messageStatus.Status == MessageStatusEnum.FAILED ? 0 : 1;
            string conversationId = "";
            string eventMessage = "";
            string pricingModel = "";
            string category = "";
            int senderId = 0;
            bool billable = false;

            if (messageStatus.PhoneNumberId != null)
            {
                if (!string.IsNullOrEmpty(messageStatus.PhoneNumberId.DisplayPhoneNumber)
                    && !string.IsNullOrEmpty(messageStatus.PhoneNumberId.PhoneNumberId))
                {
                    var senderName = await _dbContext.SenderNames.Where(x => x.ClientId == messageStatus.ClientId && x.PhoneNumberId == messageStatus.PhoneNumberId.PhoneNumberId).FirstOrDefaultAsync();
                    senderId = senderName != null ? senderName.SenderId : 0;
                }
            }

            if (messageStatus.Conversation != null)
                conversationId = messageStatus.Conversation.Id;

            if (messageStatus.Error != null)
                eventMessage = messageStatus.Error.ErrorDetails;

            if (messageStatus.Pricing != null)
            {
                pricingModel = messageStatus.Pricing.PricingModel;
                billable = messageStatus.Pricing.Billable;
                category = messageStatus.Pricing.Category;
            }

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={messageStatus.ModuleId}, @ClientId={messageStatus.ClientId}, @ParentId={messageStatus.ParentId}, @SenderId={senderId}, @PhoneNumber={messageStatus.RecipientId}, @WaId={messageStatus.WaId}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={messageStatus.UpdateDateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}, @TemplateId={messageStatus.TemplateId}, @MessageType={messageStatus.MessageType}, @MessageText={messageStatus.MessageText}, @MediaId={messageStatus.MediaId}").ToListAsync();
            return response[0];
        }
    }
}
