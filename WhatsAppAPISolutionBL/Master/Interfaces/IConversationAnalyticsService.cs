using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Conversation;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IConversationAnalyticsService
    {
        Task<ApiResult> ProcessConversationAnalyticsAsync(ConversationAnalyticRequestDto model);
        Task<List<UConversationAnalyticsData>> GetFilteredConversationsAsync(int senderId, int clientId, DateTime? startDate, DateTime? EndDate, int pageNo=0, int pageSize= int.MaxValue);
    }

}
