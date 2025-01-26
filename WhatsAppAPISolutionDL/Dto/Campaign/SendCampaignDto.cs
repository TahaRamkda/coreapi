using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Campaign
{
    public partial class SendCampaignDto
    {
        public SendCampaignDto()
        {
            PhoneNumbers = new List<string>();
        }
        public int ClientId { get; set; }
        public int CampaignId { get; set; }
        public List<string> PhoneNumbers { get; set; }
    }
}
