using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class EmployeTransaction
    {
        public long TransactionId { get; set; }
        public int? ClientId { get; set; }
        public long? EmployeId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public DateTime? NextPaydate { get; set; }
        public string? TansactionType { get; set; }
        public string? Remark { get; set; }
        public string? ReasonType { get; set; }
        public long? BankacId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
