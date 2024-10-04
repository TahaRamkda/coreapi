using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PurchaseReturnList
    {
        public long ReturnId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseListItemId { get; set; }
        public long? PurchaseId { get; set; }
        public long? DistributorProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public decimal? Price { get; set; }
        public decimal? InitialStock { get; set; }
        public bool? Isused { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? Quantity { get; set; }
        public int? MeterPerRoll { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
