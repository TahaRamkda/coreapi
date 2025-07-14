using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class AgentLogsHist
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? AgentId { get; set; }
        public int? ActionType { get; set; }
        public int? ConversationId { get; set; }
        public int? ActionBy { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }
    }
}
