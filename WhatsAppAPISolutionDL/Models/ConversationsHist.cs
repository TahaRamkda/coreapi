using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationsHist
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
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Language { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Commission { get; set; }
        public int? AgentId { get; set; }
        public int? Status { get; set; }
        public int? TotalMessages { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? LastMessageId { get; set; }
        public int? LastMessageType { get; set; }
        public string LastMessageText { get; set; }
        public int? LastMessageMediaId { get; set; }
        public int? UnreadCount { get; set; }
        public DateTime? ForceClosedDate { get; set; }
        public int? ReasonId { get; set; }
    }
}
