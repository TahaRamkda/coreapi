using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class TemplateParameterDto
    {
        public int ParamId { get; set; }
        public int? TemplatesId { get; set; }
        public int? ClientId { get; set; }
        public int? Sequence { get; set; }
        public string ParamName { get; set; }
        public int? ParamType { get; set; }
        public string ParamDefaultValue { get; set; }
        public int? Status { get; set; }
        public int? ActionBy { get; set; }
    }
}
