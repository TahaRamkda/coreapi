using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationMessage
    {
        public int MessageId { get; set; }
        public int? ConversationId { get; set; }
        public int? TypeId { get; set; }
        public int? MessageTypeId { get; set; }
        public string MessageContent { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? MessageReferenceId { get; set; }
        public string WaId { get; set; }
        public string ContextWaId { get; set; }
        public string PhoneNumber { get; set; }
        public int? MediaId { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? ParentMessageTypeId { get; set; }
        public string ParentMessageContent { get; set; }
        public int? ParentMediaId { get; set; }
        public int? ParentMessageId { get; set; }
        public string ButtonJson { get; set; }
    }
}
