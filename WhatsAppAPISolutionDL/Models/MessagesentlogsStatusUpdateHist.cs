using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class MessagesentlogsStatusUpdateHist
    {
        public int? Id { get; set; }
        public string WaId { get; set; }
        public int? EventType { get; set; }
        public DateTime? EventTime { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Message { get; set; }
    }
}
