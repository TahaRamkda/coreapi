using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplatesAnalyticsDetail
    {
        public int Id { get; set; }
        public int? AnalyticsId { get; set; }
        public string ButtonText { get; set; }
        public string ButtonType { get; set; }
        public int? ClickCount { get; set; }
    }
}
