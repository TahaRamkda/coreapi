using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class MessageReceivedLogsHist
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public int? MessageType { get; set; }
        public string MessageText { get; set; }
        public string ConversationId { get; set; }
        public string WaId { get; set; }
        public string ContextWaId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int? MediaId { get; set; }
        public decimal? Commission { get; set; }
        public DateTime? MessageSentDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
