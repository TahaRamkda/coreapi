using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Pnl
    {
        public int PnlId { get; set; }
        public int ClientId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public decimal? Profit { get; set; }
        public int? Status { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
