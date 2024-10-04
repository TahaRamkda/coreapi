using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Quotation
    {
        public long QuotationId { get; set; }
        public int ClientId { get; set; }
        public int ModuleId { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? QuotationDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string TermsAndCondition { get; set; }
        public string Validity { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public int? Status { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
