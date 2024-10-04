using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class BankAccountsTransaction
    {
        public long BankacTranId { get; set; }
        public long BankacId { get; set; }
        public int? ClientId { get; set; }
        public int? ModuleId { get; set; }
        public int? SubModuleId { get; set; }
        public long? ModuleRefId { get; set; }
        public int? TransactionType { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? PreviousBalance { get; set; }
        public decimal? Amount { get; set; }
        public decimal? CurrentBalance { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceNo2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
