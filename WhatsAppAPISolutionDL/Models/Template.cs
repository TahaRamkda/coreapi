using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Template
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string TemplateName { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public bool? IsApproved { get; set; }
        public string TemplateId { get; set; }
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
    }
}
