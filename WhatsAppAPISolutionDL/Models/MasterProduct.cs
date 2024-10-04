using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class MasterProduct
    {
        public long MasterProductId { get; set; }
        public int? ClientId { get; set; }
        public string MasterProductName { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public int? Stock { get; set; }
        public int? ProductModuleTypeId { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
