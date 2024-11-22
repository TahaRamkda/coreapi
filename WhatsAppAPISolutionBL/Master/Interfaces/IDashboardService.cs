using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IDashboardService
    {
        Task<List<UDashboardSummary>> GetDashboardSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null);

        Task<List<UDashboardReportSummary>> GetDashboardReportSummaryListAsync(int ClientId, int DashboardTypeId = 0, DateTime? FromDate = null, DateTime? ToDate = null);
    }
}
