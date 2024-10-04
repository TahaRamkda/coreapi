using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WasteProductList
    {
        public long WasteId { get; set; }
        public int? ClientId { get; set; }
        public long? WasteListItemId { get; set; }
        public long? ItemId { get; set; }
        public long? DistributorProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? WasteMeter { get; set; }
        public string QrCode { get; set; }
    }
}
