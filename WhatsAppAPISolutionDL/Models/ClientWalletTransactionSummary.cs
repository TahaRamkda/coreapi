using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ClientWalletTransactionSummary
    {
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public string TransactionType { get; set; }
        public string ConversationType { get; set; }
        public int? TotalTransactions { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Currency { get; set; }
        public decimal? TotalMetaCharges { get; set; }
        public decimal? TotalCommission { get; set; }
    }
}
