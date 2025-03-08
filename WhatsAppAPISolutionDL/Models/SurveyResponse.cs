using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyResponse
    {
        public int SurveyResponseId { get; set; }
        public int? SurveyId { get; set; }
        public int? FlowId { get; set; }
        public string MetaFlowId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string FlowToken { get; set; }
        public int? SenderId { get; set; }
        public int? ClientId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
