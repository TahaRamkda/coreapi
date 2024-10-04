using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class CommonAttribute
    {
        public long AttributeId { get; set; }
        public int? ClientId { get; set; }
        public string AttributeName { get; set; }
        public int? TagId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
