using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class DistributorProduct
    {
        public long DistributorProductId { get; set; }
        public int? ClientId { get; set; }
        public long? DistributorId { get; set; }
        public long? MasterProductId { get; set; }
        public string DistributorProductName { get; set; }
        public int? ProductModuleTypeId { get; set; }
        public string ProductCode { get; set; }
        public string ProductColor { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
