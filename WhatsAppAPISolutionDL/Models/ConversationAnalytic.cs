using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ConversationAnalytic
    {
        public int Id { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int ConversationCount { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Cost { get; set; }
    }
}
