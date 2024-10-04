using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PartialPayment
    {
        public long Id { get; set; }
        public int? PartialPaymentModuleId { get; set; }
        public int? ClientId { get; set; }
        public long? ReferenceId { get; set; }
        public long? BankacId { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
