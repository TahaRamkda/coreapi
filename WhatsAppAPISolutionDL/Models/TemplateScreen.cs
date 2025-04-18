using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplateScreen
    {
        public int TemplateScreenId { get; set; }
        public int? TemplateId { get; set; }
        public int? HeaderType { get; set; }
        public int? MediaId { get; set; }
        public int? HeaderParamCount { get; set; }
        public string HeaderText { get; set; }
        public int? BodyParamCount { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? Sequence { get; set; }
    }
}
