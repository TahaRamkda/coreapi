using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Asset
    {
        public long AssetsId { get; set; }
        public int? ClientId { get; set; }
        public string AssetsName { get; set; }
        public int? AssetsType { get; set; }
        public decimal? AssetCost { get; set; }
        public DateTime? PurchasedAt { get; set; }
        public long? BankacId { get; set; }
        public int? AssetPaymentType { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo1 { get; set; }
        public string ReferenceNo2 { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
