using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PnlBankAccount
    {
        public int Id { get; set; }
        public int PnlId { get; set; }
        public int ClientId { get; set; }
        public long? BankacId { get; set; }
        public decimal? ClosingBalance { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
