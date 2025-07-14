using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class InteractiveTemplateParametersTest
    {
        public int ParamId { get; set; }
        public int? InteractiveTemplateId { get; set; }
        public int? ClientId { get; set; }
        public string ParamName { get; set; }
        public int? PersonalizationType { get; set; }
        public string PersonalizationField { get; set; }
        public string PersonalizationDefaultValue { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
