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
        public string SendStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
