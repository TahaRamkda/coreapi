using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PrintingUsage
    {
        public long UsageId { get; set; }
        public int? ClientId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? UsageDate { get; set; }
        public string Description { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsWaste { get; set; }
    }
}
