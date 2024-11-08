using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Campaign
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string CampaignType { get; set; }
        public string Status { get; set; }
        public int? TotalContacts { get; set; }
        public int? SentCount { get; set; }
        public int? FailedCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? UndeliveredCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Commission { get; set; }
        public decimal? TotalCost { get; set; }
    }
}
