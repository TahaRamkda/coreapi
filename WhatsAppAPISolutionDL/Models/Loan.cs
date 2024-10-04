using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Loan
    {
        public long LoanId { get; set; }
        public int? ClientId { get; set; }
        public int? LoanTypeId { get; set; }
        public string LoanerName { get; set; }
        public string LoaneeName { get; set; }
        public decimal? LoanAmount { get; set; }
        public DateTime? LoanDate { get; set; }
        public string LoanPeriod { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public bool LoanRepaid { get; set; }
        public long? BankacId { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo1 { get; set; }
        public string ReferenceNo2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
