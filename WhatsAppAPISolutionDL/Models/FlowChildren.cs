using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class FlowChildren
    {
        public int FlowChildrenId { get; set; }
        public int? FlowScreenId { get; set; }
        public string ControlName { get; set; }
        public int? ControlType { get; set; }
        public string ControlText { get; set; }
        public bool? Required { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
