using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.TemplateAnalytic
{
    public class UTemplateAnalyticsSummary
    {
        public int Id { get; set; }
        public DateTime RecordDate { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public int SentCount { get; set; }
        public int DeliveredCount { get; set; }
        public int ReadCount { get; set; }
        public int FailedCount { get; set; }
        public decimal Amount_Spent { get; set; }
        public decimal CostPerDelivered { get; set; }
        public decimal CostPerUrlButtonClick { get; set; }
        public string? Details { get; set; }
    }
}
