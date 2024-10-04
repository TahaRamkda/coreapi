using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PrintingPurchaseItem
    {
        public long PurchaseItemId { get; set; }
        public long PurchaseId { get; set; }
        public int? ClientId { get; set; }
        public long? DistributorId { get; set; }
        public long? MasterProductId { get; set; }
        public long? DistributorProductId { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public int? ReceivedQuantity { get; set; }
        public int? PendingQuantity { get; set; }
        public bool? IsPartialReceived { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Total { get; set; }
        public bool? IsReturned { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
