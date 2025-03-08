using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SignalRqueue
    {
        public int Id { get; set; }
        public string SignalName { get; set; }
        public int? SignalType { get; set; }
        public int? ActionType { get; set; }
        public int? ActionId { get; set; }
        public int? ParentId { get; set; }
        public int? ModuleId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderName { get; set; }
        public int? Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
