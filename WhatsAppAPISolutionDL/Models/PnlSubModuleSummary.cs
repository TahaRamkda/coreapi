using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class PnlSubModuleSummary
    {
        public int Id { get; set; }
        public int PnlId { get; set; }
        public int ClientId { get; set; }
        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }
        public decimal? Amount { get; set; }
        public bool ImpactOnPnl { get; set; }
        public int? TransactionType { get; set; }
        public int? DisplayOrder { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
