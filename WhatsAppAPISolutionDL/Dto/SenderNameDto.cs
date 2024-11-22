using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SenderNameDto
    {
        public long SenderId { get; set; }
        public int? ClientId { get; set; }
        public string SenderName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneId { get; set; }
        public string AppId { get; set; }
        public decimal? Limit { get; set; }
        public decimal? Quality { get; set; }
        public int? ActionBy { get; set; }
    }
}
