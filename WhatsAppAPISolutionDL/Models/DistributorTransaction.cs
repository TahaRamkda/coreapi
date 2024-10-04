using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class DistributorTransaction
    {
        public long DistTranId { get; set; }
        public long DistributorId { get; set; }
        public int? ClientId { get; set; }
        public int? ModuleTypeId { get; set; }
        public long? PurchaseId { get; set; }
        public int? TransactionType { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PreviousPrintingBalance { get; set; }
        public decimal? CurrentPrintingBalance { get; set; }
        public decimal? PreviousStickerBalance { get; set; }
        public decimal? CurrentStickerBalance { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceNo2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
