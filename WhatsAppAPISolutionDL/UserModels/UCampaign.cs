using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UCampaign
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public string ScheduleDate { get; set; }
        public string Status { get; set; }
        public int? TotalContacts { get; set; }
        public int? SentCount { get; set; }
        public int? FailedCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? UndeliveredCount { get; set; }
        public string CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int? TotalItems { get; set; }
    }
}
