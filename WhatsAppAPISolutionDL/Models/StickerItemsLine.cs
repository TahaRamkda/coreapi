using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerItemsLine
    {
        public long LineId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseId { get; set; }
        public long? PurchaseItemId { get; set; }
        public long? MasterProductId { get; set; }
        public long? DistributorProductId { get; set; }
        public int? SeqNo { get; set; }
        public int? Quantity { get; set; }
        public int? Stock { get; set; }
        public decimal? CostPerMeter { get; set; }
        public int? LotNo { get; set; }
        public string VoucherNo { get; set; }
        public bool? IsReturned { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
