using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerSaleReturnList
    {
        public long ReturnId { get; set; }
        public int? ClientId { get; set; }
        public long? PurchaseListItemId { get; set; }
        public long? SaleIdId { get; set; }
        public long? ItemId { get; set; }
        public long? DistributorProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductColor { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? SellMeter { get; set; }
        public string QrCode { get; set; }
    }
}
