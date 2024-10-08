using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Template
    {
        public long TemplatesId { get; set; }
        public int? ClientId { get; set; }
        public string TemplateName { get; set; }
        public string IntegrationId { get; set; }
        public string TemplateId { get; set; }
        public int? Status { get; set; }
        public int? TemplateType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
