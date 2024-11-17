using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UAPIMessage
    {
        public long APIMessageId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderNameId { get; set; }
        public string TrxType { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public int? TemplateId { get; set; }
        public string PhoneNumber { get; set; }
        public string URL { get; set; }
        public int? Status { get; set; }
        public DateTime? ScheduleTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string WaId { get; set; }
    }
}
