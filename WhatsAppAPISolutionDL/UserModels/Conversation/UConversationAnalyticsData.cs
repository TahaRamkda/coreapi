using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class UConversationAnalyticsData
    {
        public long Start { get; set; }
        public long End { get; set; }
        public string StartUtc { get; set; }
        public string EndUtc { get; set; }
        public int ConversationCount { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Cost { get; set; }
        public string CurrencyName { get; set; }
        public string Category { get; set; }
        public string SenderName { get; set; }
        public string CreatedDate { get; set; }
        public string ConversationType { get; set; }
        public int TotalRecords { get; set; }
    }
}
