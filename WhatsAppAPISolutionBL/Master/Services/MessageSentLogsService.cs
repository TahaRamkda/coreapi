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

        public async Task<List<UMessageSentLog>> GetMessageSentLogListAsync(int ClientId, int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.MessageSentLogs.FromSqlInterpolated($"exec usp_MessageSentLogs_Ops @ActionId={(int)CrudEnum.List}, @Id={Id}, @ClientId={ClientId}, @ModuleId={ModuleId}, @ParentId={ParentId}, @PhoneNumber={PhoneNumber}, @WaId={WaId},@WaId2={WaId2},@SenderId={SenderId},@FromSentDate={FromSentDate}, @ToSentDate={ToSentDate}, @FromDeliveredDate={FromDeliveredDate}, @ToDeliveredDate={ToDeliveredDate}, @FromReadDate={FromReadDate}, @ToReadDate={ToReadDate},@FromDate={FromDate}, @ToDate={ToDate}, @CurrentStatus={CurrentStatus}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNumber={PageNo}, @PageSize={PageSize}").ToListAsync();

            return response;
        }
    }
}
