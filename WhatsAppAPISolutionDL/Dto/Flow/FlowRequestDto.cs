using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto.Flow
{
    public class FlowRequestDto
    {
        public string ClientId { get; set; }
        public string SenderNameId { get; set; }
        public string FlowId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string EndpointUrl { get; set; }
        public string FlowJson { get; set; }
    }
    public class PublishFlowRequestDto
    {
        public string ClientId { get; set; }
        public string SenderNameId { get; set; }
        public string FlowId { get; set; }
    }
}
