using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class TemplateResponsesHist
    {
        public int Id { get; set; }
        public int? TemplateId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public int? ResponseType { get; set; }
        public string ResponseText { get; set; }
        public string PhoneNumber { get; set; }
        public string WaId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
