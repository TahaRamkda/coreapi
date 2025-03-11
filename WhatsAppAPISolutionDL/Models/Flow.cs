using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Flow
    {
        public int FlowId { get; set; }
        public string MetaFlowId { get; set; }
        public string MetaFlowName { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public string FlowName { get; set; }
        public string FlowLanguage { get; set; }
        public string FlowJson { get; set; }
        public string Status { get; set; }
        public bool? IsPublished { get; set; }
        public string DataApiVersion { get; set; }
        public string Version { get; set; }
        public string EndpointUrl { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? ActionId { get; set; }
        public int? ActionType { get; set; }
    }
}
