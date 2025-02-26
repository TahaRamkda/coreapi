using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class AgentChatReasonMap
    {
        public int Id { get; set; }
        public int? AgentId { get; set; }
        public int? ChatReasonId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
