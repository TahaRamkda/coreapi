using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationsAgentSummary
    {
        public int? ClientId { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? ActiveChat { get; set; }
        public int? AssignedChat { get; set; }
        public int? UnAssignedChat { get; set; }
        public int? AbandonChat { get; set; }
        public int? ExpiredChat { get; set; }
        public int? ForceClosedChat { get; set; }
        public int? ClosedChat { get; set; }
        public int? MaxEligibleChat { get; set; }
        public decimal? Rating { get; set; }
        public int? AvgResponseTime { get; set; }
        public int? AvgChatTime { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TimeZone { get; set; }
        public int? IsDisabled { get; set; }
    }
}
