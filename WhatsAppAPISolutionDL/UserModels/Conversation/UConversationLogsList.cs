using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class UConversationLogsList : UListEntity
    {
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public int? AgentId { get; set; }
        public string AgentFullName { get; set; }
        public string UpdatedDate { get; set; }
    }
}
