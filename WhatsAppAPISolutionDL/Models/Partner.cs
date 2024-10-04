using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Partner
    {
        public long PartnerId { get; set; }
        public int? ClientId { get; set; }
        public string PartnerName { get; set; }
        public DateTime? JoinedDate { get; set; }
        public decimal? SplitPercent { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public decimal? CurrentInvestment { get; set; }
    }
}
