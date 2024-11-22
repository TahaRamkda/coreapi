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

        public async Task<List<UDashboardSummary>> GetDashboardSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var response = await _dbContext2.DashboardSummary.FromSqlInterpolated($"exec usp_Dashboard_Ops  @ClientId={ClientId}, @DashboardTypeId={DashboardTypeId}, @FromDate={FromDate}, @ToDate={ToDate}").ToListAsync();
            return response;
        }

        public async Task<List<UDashboardReportSummary>> GetDashboardReportSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            if (!FromDate.HasValue && !ToDate.HasValue)
                return new List<UDashboardReportSummary>();

            var response = await _dbContext2.DashboardReportSummary.FromSqlInterpolated($"exec usp_Dashboard_Report_Ops @ClientId={ClientId}, @DashboardTypeId={DashboardTypeId}, @FromDate={FromDate}, @ToDate={ToDate}").ToListAsync();
            return response;
        }

    }
}
