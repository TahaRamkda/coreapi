using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Models
{
    public class StatusUpdateModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int MessageId { get; set; }
        public int AgentId { get; set; }
        public string EventMessage { get; set; }
        public int MessageStatus { get; set; }
    }
}
