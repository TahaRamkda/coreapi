using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ProductStock
    {
        public long StockId { get; set; }
        public int? ClientId { get; set; }
        public long? ItemId { get; set; }
        public int? EntryType { get; set; }
        public long? EntryId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? CurrentStock { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
