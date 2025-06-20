using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Dto.TemplateAnalytics;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ITemplateAnalyticsService
    {
        Task<ApiResult> ProcessTemplateAnalyticsAsync(TemplateAnalyticsRequestDto model);
    }
}
