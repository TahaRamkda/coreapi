using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IDashboardService
    {
        Task<string> GetDashboardSummaryAsync(int clientId, int senderId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<string> GetTemplateInsightAsync(int clientId, int templateId = 0, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
