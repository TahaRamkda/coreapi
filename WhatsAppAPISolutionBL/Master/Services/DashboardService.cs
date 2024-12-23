using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public DashboardService(WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<string> GetDashboardSummaryAsync(int clientId, int senderId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var response = await _dbContext2.JsonDatas.FromSqlInterpolated($"exec usp_Dashboard_Ops  @ClientId={clientId}, @SenderId={senderId}, @DashboardTypeId=1, @FromDate={fromDate}, @ToDate={toDate}").ToListAsync();
            if (response != null && response.Any())
                return response[0].JsonDataStr;

            return String.Empty;
        }

        public async Task<string> GetTemplateInsightAsync(int clientId, int templateId = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var response = await _dbContext2.JsonDatas.FromSqlInterpolated($"exec usp_Template_Insight  @ClientId={clientId}, @TemplateId={templateId}, @FromDate={fromDate}, @ToDate={toDate}").ToListAsync();
            if (response != null && response.Any())
                return response[0].JsonDataStr;

            return String.Empty;
        }
    }
}
