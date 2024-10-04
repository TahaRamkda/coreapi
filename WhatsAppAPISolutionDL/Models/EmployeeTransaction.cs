using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class EmployeeTransaction
    {
        public long TransactionId { get; set; }
        public long EmployeeId { get; set; }
        public int? ClientId { get; set; }
        public long? BankacId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PreviousBalanceAmount { get; set; }
        public decimal? CurrentBalanceAmount { get; set; }
        public decimal? PreviousAdvanceAmount { get; set; }
        public decimal? CurrentAdvanceAmount { get; set; }
        public int? ReasonType { get; set; }
        public int? TransactionType { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceNo2 { get; set; }
        public DateTime? NextPaydate { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
