using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PurchaseItem
    {
        public long ItemId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseListItemId { get; set; }
        public string ItemQr { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public long? RollNo { get; set; }
        public long? TotalRoll { get; set; }
        public decimal? InitialStock { get; set; }
        public decimal? Price { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
