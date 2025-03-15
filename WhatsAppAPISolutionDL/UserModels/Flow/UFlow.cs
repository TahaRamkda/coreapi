using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Flow
{
    public partial class UFlow : UEntity
    {
        public int FlowId { get; set; }
        public string MetaFlowId { get; set; }
        public string MetaFlowName { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string SenderName { get; set; }
        public int? SenderId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public string FlowName { get; set; }
        public string FlowLanguage { get; set; }
        public string Status { get; set; }
        public bool? IsPublished { get; set; }
    }
}
