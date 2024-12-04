using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class APIMessageDto
    {
        public int? ClientId { get; set; }
        public int? SenderNameId { get; set; }
        public string TrxType { get; set; }
        public string Udf1 { get; set; }
        public string Udf2 { get; set; }
        public int? TemplateId { get; set; }
        public string PhoneNumber { get; set; }
        public string Url { get; set; }
        public int? Status { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public string WaId { get; set; }
        public int? ActionBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
