using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class FlowOption
    {
        public int Id { get; set; }
        public int ScreenChildrenId { get; set; }
        public string OptionId { get; set; }
        public string OptionText { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
