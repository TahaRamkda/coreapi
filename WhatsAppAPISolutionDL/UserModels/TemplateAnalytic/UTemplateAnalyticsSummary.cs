using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.TemplateAnalytic
{
    public class UTemplateAnalyticsSummary
    {
        public int TotalSent { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalRead { get; set; }
        public int TotalFailed { get; set; }
        public decimal TotalAmountSpent { get; set; }
        public decimal TotalCostPerDelivered { get; set; }
        public decimal TotalCostPerUrlButtonClick { get; set; }
        public string? Details { get; set; } // JSON string
    }
}
