using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MessageSentLogsService : IMessageSentLogsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<MessageSentLogsService> _logger;
        private readonly ISenderNameService _senderNameService;

        public MessageSentLogsService(WhatsAppSolutionContext dbContext, 
            WhatsAppSolutionContext2 dbContext2, 
            ILogger<MessageSentLogsService> logger,
            ISenderNameService senderNameService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _senderNameService = senderNameService;
        }

        public async Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int ClientId, int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.MessageSentLogs.FromSqlInterpolated($"exec usp_MessageSentLogs_Ops @ActionId={(int)CrudEnum.List}, @Id={Id}, @ClientId={ClientId}, @ModuleId={ModuleId}, @ParentId={ParentId}, @PhoneNumber={PhoneNumber}, @WaId={WaId},@WaId2={WaId2},@SenderId={SenderId},@FromSentDate={FromSentDate}, @ToSentDate={ToSentDate}, @FromDeliveredDate={FromDeliveredDate}, @ToDeliveredDate={ToDeliveredDate}, @FromReadDate={FromReadDate}, @ToReadDate={ToReadDate},@FromDate={FromDate}, @ToDate={ToDate}, @CurrentStatus={CurrentStatus}, @SearchStr={SearchStr ?? ""}, @SortBy={SortBy}, @PageNumber={PageNo}, @PageSize={PageSize}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_MessageSentLogs_Ops with parameters: " +
                "ActionId={ActionId}, ActionName={ActionName}, ClientId={ClientId}, Id={Id}, ModuleId={ModuleId}, ParentId={ParentId}, PhoneNumber={PhoneNumber}, WaId={WaId}, WaId2={WaId2}, SenderId={SenderId}, " +
                "FromSentDate={FromSentDate}, ToSentDate={ToSentDate}, FromDeliveredDate={FromDeliveredDate}, ToDeliveredDate={ToDeliveredDate}, FromReadDate={FromReadDate}, ToReadDate={ToReadDate}, " +
                "FromDate={FromDate}, ToDate={ToDate}, CurrentStatus={CurrentStatus}, SearchStr={SearchStr}, SortBy={SortBy}, PageNumber={PageNo}, PageSize={PageSize}, " +
                "ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.List, CrudEnum.List, ClientId, Id, ModuleId, ParentId, PhoneNumber, WaId, WaId2, SenderId, FromSentDate, ToSentDate, FromDeliveredDate, ToDeliveredDate, FromReadDate, ToReadDate, FromDate, ToDate, CurrentStatus, SearchStr ?? "", SortBy, PageNo, PageSize, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<UResponse> AddMessageSentLogAsync(InsertMessageDto model)
        {

            int eventType = (int)model.Status;
            int eventStatus = model.Status == MessageStatusEnum.FAILED ? 0 : 1;
            string conversationId = "";
            string eventMessage = "";
            string pricingModel = "";
            string category = "";
            bool billable = false;

            if (model.PhoneNumberId != null && model.SenderId == 0) //If sender id is not available, search via phone number id
            {
                if (!string.IsNullOrEmpty(model.PhoneNumberId.DisplayPhoneNumber)
                    && !string.IsNullOrEmpty(model.PhoneNumberId.PhoneNumberId))
                {
                    var senderName = await _senderNameService.GetSenderNameEntityByPhoneNumberIdAsync(model.PhoneNumberId.PhoneNumberId); 
                    model.SenderId = senderName != null ? senderName.SenderId : 0;
                }
            }

            if (model.Conversation != null)
                conversationId = model.Conversation.Id;

            if (model.Error != null)
                eventMessage = model.Error.ErrorDetails;

            if (model.Pricing != null)
            {
                pricingModel = model.Pricing.PricingModel;
                billable = model.Pricing.Billable;
                category = model.Pricing.Category;
            }

            _logger.LogInformation("Calling procedure usp_MessageSentLogs_StatusUpdate with request={request}", $"exec usp_MessageSentLogs_StatusUpdate @ModuleId={model.ModuleId}, @ClientId={model.ClientId}, @ParentId={model.ParentId}, @SenderId={model.SenderId}, @PhoneNumber={model.RecipientId}, @WaId={model.WaId}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={model.UpdateDateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}, @MessageReferenceId={model.MessageReferenceId}, @MessageType={model.MessageType}, @MessageContent={model.MessageContent}, @MediaId={model.MediaId}, @ButtonJson={model.ButtonJson}, @SystemGenerated={model.SystemGenerated}");

            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_MessageSentLogs_StatusUpdate @ModuleId={model.ModuleId}, @ClientId={model.ClientId}, @ParentId={model.ParentId}, @SenderId={model.SenderId}, @PhoneNumber={model.RecipientId}, @WaId={model.WaId}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={model.UpdateDateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}, @MessageReferenceId={model.MessageReferenceId}, @MessageType={model.MessageType}, @MessageContent={model.MessageContent}, @MediaId={model.MediaId}, @ButtonJson={model.ButtonJson}, @SystemGenerated={model.SystemGenerated}").ToListAsync();

            _logger.LogInformation("Calling procedure usp_MessageSentLogs_StatusUpdate with request={request} and response={response} and ProcResponseTime={ProcResponseTime}", $"exec usp_MessageSentLogs_StatusUpdate @ModuleId={model.ModuleId}, @ClientId={model.ClientId}, @ParentId={model.ParentId}, @SenderId={model.SenderId}, @PhoneNumber={model.RecipientId}, @WaId={model.WaId}, @WaId2={conversationId}, @EventType={eventType}, @EventTime={model.UpdateDateTime}, @EventStatus={eventStatus}, @EventMessage={eventMessage}, @PricingModel={pricingModel}, @Billable={billable}, @Category={category}, @MessageReferenceId={model.MessageReferenceId}, @MessageType={model.MessageType}, @MessageContent={model.MessageContent}, @MediaId={model.MediaId}, @ButtonJson={model.ButtonJson},@UDF1={model.UDF1} ,@UDF2={model.UDF2}", JsonConvert.SerializeObject(response), DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            return response[0];
        }
    }
}
