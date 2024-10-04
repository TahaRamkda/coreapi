using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class InvestmentTransaction
    {
        public long ItransactionId { get; set; }
        public int? ClientId { get; set; }
        public long? PartnerId { get; set; }
        public int? TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? CurrentInvestment { get; set; }
        public decimal? PreviousInvestment { get; set; }
        public DateTime? InvestmentDate { get; set; }
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
