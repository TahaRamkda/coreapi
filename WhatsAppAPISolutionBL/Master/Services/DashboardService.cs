using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly WhatsAppAPISolutionContext2 _dbContext2;

        public DashboardService(WhatsAppAPISolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<List<UDashboardSummary>> GetDashboardSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var response = await _dbContext2.DashboardSummary.FromSqlInterpolated($"exec usp_Dashboard_Ops  @Client_Id={client_Id}, @Dashboard_TypeId={dashboardType_Id}, @From_Date={fromDate}, @To_Date={toDate}").ToListAsync();
            return response;
        }

        public async Task<List<UDashboardReportSummary>> GetDashboardReportSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!fromDate.HasValue && !toDate.HasValue)
                return new List<UDashboardReportSummary>();

            var response = await _dbContext2.DashboardReportSummary.FromSqlInterpolated($"exec usp_Dashboard_Report_Ops @Client_Id={client_Id}, @Dashboard_TypeId={dashboardType_Id}, @From_Date={fromDate}, @To_Date={toDate}").ToListAsync();
            return response;
        }

    }
}
