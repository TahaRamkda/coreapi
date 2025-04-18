using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class UConversationAnalytics
    {
        public int Id { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int ConversationCount { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Cost { get; set; }
        public string Category { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
