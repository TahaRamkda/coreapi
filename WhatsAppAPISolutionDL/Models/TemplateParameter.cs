using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplateParameter
    {
        public long ParamId { get; set; }
        public long? TemplatesId { get; set; }
        public int? ClientId { get; set; }
        public int? Sequence { get; set; }
        public string ParamName { get; set; }
        public string ParamText { get; set; }
        public int? ParamType { get; set; }
        public string ParamDefaultValue { get; set; }
        public bool? IsDynamic { get; set; }
        public int? ButtonType { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? IsContactLevel { get; set; }
        public string ContactLevelField { get; set; }
    }
}
