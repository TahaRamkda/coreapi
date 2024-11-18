using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IMessageSentLogsService
    {
        Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int clientId, int id = 0, int moduleId = 0, int parentId = 0, string phoneNumber = "", string waId = "", string waId2 = "", int senderId = 0, DateTime? fromSentDate = null, DateTime? toSentDate = null, DateTime? fromDeliveredDate = null, DateTime? toDeliveredDate = null, DateTime? fromReadDate = null, DateTime? toReadDate = null, DateTime? fromDate = null, DateTime? toDate = null, int currentStatus = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue);
    }
}
