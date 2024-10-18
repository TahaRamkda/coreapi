using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplateParametersConfig
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public int? ParamSeq { get; set; }
        public int? ParamType { get; set; }
        public string ParamName { get; set; }
        public string ParamText { get; set; }
    }
}
