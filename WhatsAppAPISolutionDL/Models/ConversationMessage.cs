using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationMessage
    {
        public long MessageId { get; set; }
        public string ConversationId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int? MessageTypeId { get; set; }
        public string MessageText { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
