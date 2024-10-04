using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class LoanTransaction
    {
        public long TransactionId { get; set; }
        public int? ClientId { get; set; }
        public long? LoanId { get; set; }
        public long? BankacId { get; set; }
        public decimal? PreviousAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Remarks { get; set; }
        public string ReferanceNo1 { get; set; }
        public string ReferanceNo2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
