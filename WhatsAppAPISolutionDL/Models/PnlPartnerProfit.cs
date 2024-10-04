using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PnlPartnerProfit
    {
        public int Id { get; set; }
        public int PnlId { get; set; }
        public int ClientId { get; set; }
        public long? PartnerId { get; set; }
        public decimal? Profit { get; set; }
        public decimal? SplitPercent { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
