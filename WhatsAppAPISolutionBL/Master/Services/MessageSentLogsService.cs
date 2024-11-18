using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MessageSentLogsService: IMessageSentLogsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public MessageSentLogsService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int clientId, int id = 0, int moduleId = 0, int parentId = 0, string phoneNumber = "", string waId = "", string waId2 = "", int senderId = 0, DateTime? fromSentDate = null, DateTime? toSentDate = null, DateTime? fromDeliveredDate = null, DateTime? toDeliveredDate = null, DateTime? fromReadDate = null, DateTime? toReadDate = null, DateTime? fromDate = null, DateTime? toDate = null, int currentStatus = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.MessageSentLogs.FromSqlInterpolated($"exec usp_MessageSentLogs_Ops @ActionId={(int)CrudEnum.List}, @Id={id}, @ClientId={clientId}, @ModuleId={moduleId}, @ParentId={parentId}, @PhoneNumber={phoneNumber}, @WaId={waId},@WaId2={waId2},@SenderId={senderId},@FromSentDate={fromSentDate}, @ToSentDate={toSentDate}, @FromDeliveredDate={fromDeliveredDate}, @ToDeliveredDate={toDeliveredDate}, @FromReadDate={fromReadDate}, @ToReadDate={toReadDate},@FromDate={fromDate}, @ToDate={toDate}, @CurrentStatus={currentStatus}, @SearchStr={searchStr}, @SortBy={sortBy}, @PageNumber={pageNo}, @PageSize={pageSize}").ToListAsync();

            return response;
        }
    }
}
