using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class AgentDto
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string AgentFName { get; set; }
        public string AgentLName { get; set; }
        public string SenderIds { get; set; }
        public int? ActionBy { get; set; }
    }
}
