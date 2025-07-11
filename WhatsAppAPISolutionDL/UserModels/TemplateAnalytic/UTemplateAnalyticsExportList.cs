using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.TemplateAnalytic
{
    public class UTemplateAnalyticsExportList
    {
        public int ClientId { get; set; }
        public string TemplateName { get; set; }
        public string SenderName { get; set; }
        public int SentCount { get; set; }
        public int DeliveredCount { get; set; }
        public int ReadCount { get; set; }
        public int FailedCount { get; set; }
        public string ButtonText { get; set; }
        public string ButtonType { get; set; }
        public int? ClickCount { get; set; }
    }
}
