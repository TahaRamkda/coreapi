using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Medium
    {
        public long Id { get; set; }
        public int? ClientId { get; set; }
        public string WhatsAppBusinessAccountId { get; set; }
        public int? SenderNameId { get; set; }
        public string MediaUrl { get; set; }
        public string MediaId { get; set; }
        public string MediaPath { get; set; }
        public int? RecordStatus { get; set; }
    }
}
