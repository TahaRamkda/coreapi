using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerPurchase
    {
        public long PurchaseId { get; set; }
        public int? ClientId { get; set; }
        public string InvoiceNo { get; set; }
        public long? DistributorId { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public bool? IsPartialReceived { get; set; }
        public decimal? SettlementAmount { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
