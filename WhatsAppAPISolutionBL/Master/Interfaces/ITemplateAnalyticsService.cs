using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateAnalyticsService
    {
        Task<ApiResult> ProcessTemplateAnalyticsAsync(TemplateAnalyticsRequestDto model);
        Task<List<UTemplateAnalyticsSummary>> GetAnalyticsSummaryAsync(int clientId,int senderId,string templateId, DateTime? startDate, DateTime? endDate);
        Task<List<UTemplateAnalyticsDetailsList>> GetAnalyticsDetailsList(int clientId,int senderId, DateTime? startDate, DateTime? endDate, string templateId = null);
        Task<List<UTemplateAnalyticsExportList>> GetExportAnalyticsDetailsList(int clientId,int senderId, DateTime? startDate, DateTime? endDate, string templateId = null, string frequency=null);
    }
}
