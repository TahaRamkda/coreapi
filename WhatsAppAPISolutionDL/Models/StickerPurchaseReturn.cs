using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerPurchaseReturn
    {
        public long ReturnId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseId { get; set; }
        public long? InvoiceNo { get; set; }
        public long? DistributorId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public bool? IsPartial { get; set; }
        public DateTime? NextReceivedDate { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
