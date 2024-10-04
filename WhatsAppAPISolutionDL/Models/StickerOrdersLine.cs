using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerOrdersLine
    {
        public long OrderLine { get; set; }
        public long OrderId { get; set; }
        public int? ClientId { get; set; }
        public long LineId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Rate { get; set; }
        public decimal? CostPerMeter { get; set; }
        public decimal? ProfitPerMeter { get; set; }
        public decimal? Total { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
