using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class FlowScreen
    {
        public int FlowScreenId { get; set; }
        public int? FlowId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string ScreenButtonText { get; set; }
        public int? RedirectionType { get; set; }
        public string RedirectionScreen { get; set; }
        public string Payload { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
