using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IConversationAnalyticsService
    {
        Task<ApiResult> ProcessConversationAnalyticsAsync(ConversationAnalyticRequestDto model);
    }

}
