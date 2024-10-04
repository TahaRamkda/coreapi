using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PrintingUsageLine
    {
        public long UsageLineId { get; set; }
        public long UsageId { get; set; }
        public int? ClientId { get; set; }
        public long LineId { get; set; }
        public int? Quantity { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
