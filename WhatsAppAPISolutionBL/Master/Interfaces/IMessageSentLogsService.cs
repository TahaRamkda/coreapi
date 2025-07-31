using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMessageSentLogsService
    {
        Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int ClientId, int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);

        public Task<UResult> AddMessageSentLogAsync(InsertMessageDto messageStatus);
    }
}
