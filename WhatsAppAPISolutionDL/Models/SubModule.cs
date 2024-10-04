using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SubModule
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int SubModuleId { get; set; }
        public string SubModuleCode { get; set; }
        public string SubModuleName { get; set; }
        public bool ImpactOnPnl { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? TransactionType { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
