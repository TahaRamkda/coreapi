using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PrintingOrder
    {
        public long OrderId { get; set; }
        public int? ClientId { get; set; }
        public string InvoiceNo { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? OtherCharges { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string CancellationRemarks { get; set; }
        public long? PaidFromBankacId { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
