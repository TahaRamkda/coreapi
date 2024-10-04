using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class BankAccount
    {
        public long BankacId { get; set; }
        public int? ClientId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Iban { get; set; }
        public string BranchCode { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CurrentBalance { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
