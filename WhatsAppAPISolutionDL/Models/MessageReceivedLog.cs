using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class MessageReceivedLog
    {
        public long Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ModuleId { get; set; }
        public long? ParentId { get; set; }
        public string ConversationId { get; set; }
        public string WaId { get; set; }
        public string ContextWaId { get; set; }
        public string PhoneNumber { get; set; }
        public int? ReponseType { get; set; }
        public string ResponseText { get; set; }
        public decimal? Commission { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? MessageSentDate { get; set; }
    }
}
