using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
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
            int eventType = (int)messageStatus.status;
            int eventStatus = messageStatus.status == MessageStatusEnum.FAILED ? 0 : 1;
            string conversationId = "";
            string eventMessage = "";
            string pricingModel = "";
            string category = "";
            long senderId = 0;
            bool billable = false;

            if (messageStatus.phone_number_Id != null)
            {
                if (!string.IsNullOrEmpty(messageStatus.phone_number_Id.display_phone_number)
                    && !string.IsNullOrEmpty(messageStatus.phone_number_Id.phone_number_id))
                {
                    var senderName = await _dbContext.SenderNames.Where(x => x.ClientId == messageStatus.client_Id && x.PhoneNumberId == messageStatus.phone_number_Id.phone_number_id).FirstOrDefaultAsync();
                    senderId = senderName != null ? senderName.SenderId : 0;
                }
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

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={messageStatus.module_Id}, @ClientId={messageStatus.client_Id}, @ParentId={messageStatus.parent_Id}, @SenderId={senderId}, @PhoneNumber={messageStatus.recipient_Id}, @WaId={messageStatus.wam_Id}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={messageStatus.update_dateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}, @TemplateId={messageStatus.template_Id}, @MessageType={messageStatus.message_Type}, @MessageText={messageStatus.message_Text}").ToListAsync();
            return response[0];
        }
    }
}
