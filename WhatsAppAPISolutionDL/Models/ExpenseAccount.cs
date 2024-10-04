using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ExpenseAccount
    {
        public long ExpenseId { get; set; }
        public int? ClientId { get; set; }
        public string ExpenseName { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? BankacId { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string ReferenceNo { get; set; }
        public int? RecordStatus { get; set; }
    }
}
