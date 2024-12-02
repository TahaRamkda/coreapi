using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class ActivateCampaignDto
    {
        public int CampaignId { get; set; }
        public int ClientId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int? ActionBy { get; set; }
    }
}
