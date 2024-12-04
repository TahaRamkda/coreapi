using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Conversation
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string ConversationId { get; set; }
        public string WaId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public int? ConversationType { get; set; }
        public string ConversationMode { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Commission { get; set; }
        public int? AgentId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
