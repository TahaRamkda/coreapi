using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PrintingQuotation
    {
        public long QuotationId { get; set; }
        public int? ClientId { get; set; }
        public long? CustomerId { get; set; }
        public decimal? Discount { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public DateTime? QuotationDate { get; set; }
        public int? ValidDays { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
