using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class AssetsTransactionDetail
    {
        public long TransactionId { get; set; }
        public int? ClientId { get; set; }
        public long? AssetsId { get; set; }
        public long? PartnerId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
    }
}
