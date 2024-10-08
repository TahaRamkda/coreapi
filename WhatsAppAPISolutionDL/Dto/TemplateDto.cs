using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class TemplateDto
    {
        public long Templates_Id { get; set; }
        public int? Client_Id { get; set; }
        public string Template_Name { get; set; }
        public string Integration_Id { get; set; }
        public string Template_Id { get; set; }
        public int? Status { get; set; }
        public int? Template_Type { get; set; }
        public int? ActionBy { get; set; }
    }
}
