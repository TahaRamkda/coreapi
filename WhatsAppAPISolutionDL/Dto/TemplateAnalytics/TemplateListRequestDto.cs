using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.TemplateAnalytics
{
    public class TemplateListRequestDto
    {
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
