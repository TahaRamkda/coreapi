using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class StickerSalesReturn
    {
        public long SaleReturnId { get; set; }
        public long? ClientId { get; set; }
        public long? SaleId { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? SaleDate { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
