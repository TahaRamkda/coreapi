using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SystemAction
    {
        public int SystemActionId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string ActionName { get; set; }
        public string ThirdPartyUrl { get; set; }
        public int? ActionType { get; set; }
        public int? ActionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public string TaskInfo { get; set; }
    }
}
