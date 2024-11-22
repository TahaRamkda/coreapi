using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Apiresponse
    {
        public long Id { get; set; }
        public long? ClientId { get; set; }
        public long? SenderId { get; set; }
        public string ApimessageId { get; set; }
        public long? TemplateId { get; set; }
        public string Text { get; set; }
        public string WaId { get; set; }
        public string ContextWaId { get; set; }
        public string PhoneNumber { get; set; }
        public int? ActionType { get; set; }
        public int? ActionId { get; set; }
        public int? ResponseType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
