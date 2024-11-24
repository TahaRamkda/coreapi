using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UTemplateParameter
    {
        public long ParamId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int Sequence { get; set; }
        public string ParamName { get; set; }
        public int? ParamType { get; set; }
        public string ParamDefaultValue { get; set; }
        public bool IsDynamic { get; set; }
        public int? ButtonType { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        //public int? Total { get; set; }
    }
}
