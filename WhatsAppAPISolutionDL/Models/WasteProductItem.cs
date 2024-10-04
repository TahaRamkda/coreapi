using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WasteProductItem
    {
        public long WasteId { get; set; }
        public int? ClientId { get; set; }
        public long? WasteListItemId { get; set; }
        public long? DistributorProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public decimal? Price { get; set; }
        public decimal? InitialStock { get; set; }
        public bool? IsUsed { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? Quantity { get; set; }
        public int? MeterPerRoll { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
