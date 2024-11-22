using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SendCampaignDto
    {
        public long CampaignId { get; set; }
        public string CampaignName { get; set; }
        public long ClientId { get; set; }
        public long SenderId { get; set; }
        public long TemplateId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int CampaignType {  get; set; }
        public long GroupId {  get; set; }
        public string Status {  get; set; }
        public int TotalContacts {  get; set; }
    }
}
