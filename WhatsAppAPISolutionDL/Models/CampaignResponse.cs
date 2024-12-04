using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class CampaignResponse
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? CampaignId { get; set; }
        public int? TemplateId { get; set; }
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
