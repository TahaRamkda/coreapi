using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class TemplateParameterDto
    {
        public long Param_Id { get; set; }
        public long? Templates_Id { get; set; }
        public int? Client_Id { get; set; }
        public int? Sequence { get; set; }
        public string Param_Name { get; set; }
        public int? Param_Type { get; set; }
        public string Param_Default_Value { get; set; }
        public int? Status { get; set; }
        public int? ActionBy { get; set; }
    }
}
