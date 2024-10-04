using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerSellTransaction
    {
        public long TransactionId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseId { get; set; }
        public long? BankacId { get; set; }
        public string RefranceNo1 { get; set; }
        public string RefranceNo2 { get; set; }
        public decimal? Amount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
