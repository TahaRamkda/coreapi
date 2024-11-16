using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SendSmsDto
    {
        public string Phone { get; set; }
        public string BrandName { get; set; }
        public double Amount { get; set; }
        public string OrderId { get; set; }
        public string TemplateName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
