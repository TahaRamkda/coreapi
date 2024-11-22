using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationLog
    {
        public long Id { get; set; }
        public long? ConversationId { get; set; }
        public int? ActionType { get; set; }
        public int? ActionBy { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
