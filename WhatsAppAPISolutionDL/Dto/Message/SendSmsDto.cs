using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Message
{
    public partial class SendSmsDto
    {
        public string PhoneNumber { get; set; }
        public string BrandName { get; set; }
        public string TemplateName { get; set; }
        public string HParam { get; set; }
        public string BParam1 { get; set; }
        public string BParam2 { get; set; }
        public string BParam3 { get; set; }
        public string BParam4 { get; set; }
        public string BtnParam1 { get; set; }
        public string BtnParam2 { get; set; }
        public string BtnParam3 { get; set; }
        public string BtnParam4 { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ParamJson { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public bool IsForceSend { get; set; }
    }
}
