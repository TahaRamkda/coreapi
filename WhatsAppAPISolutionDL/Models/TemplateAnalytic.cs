using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplateAnalytic
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string TemplateId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? SentCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? ReadCount { get; set; }
        public int? FailedCount { get; set; }
        public decimal? AmountSpent { get; set; }
        public decimal? CostPerDelivered { get; set; }
        public decimal? CostPerUrlButtonClick { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
