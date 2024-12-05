using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class AgentSenderMap
    {
        public int Id { get; set; }
        public int? AgentId { get; set; }
        public int? SenderId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
