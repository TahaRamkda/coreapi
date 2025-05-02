using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ModifierItemMap
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ItemId { get; set; }
        public int? ModifierGroupId { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? DeprecatedDate { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IsDefault { get; set; }
    }
}
