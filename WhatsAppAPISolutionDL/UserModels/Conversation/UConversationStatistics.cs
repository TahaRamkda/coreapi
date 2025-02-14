using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class UConversationStatistics
    {
        public int? TotalConversation { get; set; }
        public int? MarketingConversation { get; set; }
        public int? UtilityConversation { get; set; }
        public int? InitiatedConversation { get; set; }
        public int? ForceClosed { get; set; }
        public int? Closed { get; set; }
        public int? Abandon { get; set; }
        public int? LookingforAgent { get; set; }
        public int? Ongoing { get; set; }
    }
}
