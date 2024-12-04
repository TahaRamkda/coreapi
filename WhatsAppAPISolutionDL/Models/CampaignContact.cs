using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class CampaignContact
    {
        public int CampaignNumberId { get; set; }
        public int? CampaignId { get; set; }
        public int? GroupId { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Area { get; set; }
        public decimal? Cost { get; set; }
        public int? SendStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string WaId { get; set; }
        public DateTime? SentTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public DateTime? ReadTime { get; set; }
    }
}
