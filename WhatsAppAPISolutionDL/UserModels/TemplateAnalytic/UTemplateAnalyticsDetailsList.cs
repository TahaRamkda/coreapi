using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.TemplateAnalytic
{
    public class UTemplateAnalyticsDetailsList
    {
        public int? ClientId { get; set; }
        public string TemplateId { get; set; } 
        public string TemplateName { get; set; } 
        public int? SenderId { get; set; }
        public int? SentCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? ReadCount { get; set; }
        public int? FailedCount { get; set; }
        public string ButtonDetails { get; set; }
    }
}
