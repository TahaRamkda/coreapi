using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class MessageSentLog
    {
        public long Id { get; set; }
        public long? ModuleId { get; set; }
        public long? ParentId { get; set; }
        public int? TemplateId { get; set; }
        public string PhoneNumber { get; set; }
        public string WaId { get; set; }
        public string WaId2 { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public DateTime? SentDate { get; set; }
        public int? SentStatus { get; set; }
        public string SentMessage { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public int? DeliveredStatus { get; set; }
        public string DeliveredMessage { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? ReadStatus { get; set; }
        public string ReadMessage { get; set; }
        public string PricingModel { get; set; }
        public bool? Billable { get; set; }
        public string MessageText { get; set; }
        public int? MessageType { get; set; }
        public string Category { get; set; }
        public decimal? EstPrice { get; set; }
        public decimal? Commission { get; set; }
        public int? CurrentStatus { get; set; }
        public string FailedMessage { get; set; }
        public DateTime? FailedTime { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
