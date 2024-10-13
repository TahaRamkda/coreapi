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
        public string Category { get; set; }
        public string Sub_Category { get; set; }
        public string Integration_Id { get; set; }
        public string Template_Id { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public bool IsApproved { get; set; }
        public int Header_Type { get; set; }
        public int Header_Param_Count { get; set; }
        public string Header_Text { get; set; }
        public int? Template_Type { get; set; }
        public int? ActionBy { get; set; }
    }
}
