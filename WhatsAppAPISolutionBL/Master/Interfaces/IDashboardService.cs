using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IDashboardService
    {
        Task<List<UDashboardSummary>> GetDashboardSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null);

        Task<List<UDashboardReportSummary>> GetDashboardReportSummaryListAsync(int client_Id, int dashboardType_Id = 0, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
