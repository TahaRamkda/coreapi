using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyResponse
    {
        public long SurveyResponseId { get; set; }
        public int? SurveyId { get; set; }
        public int? FlowId { get; set; }
        public string MetaFlowId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string FlowToken { get; set; }
        public string SenderId { get; set; }
        public string ClientId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
