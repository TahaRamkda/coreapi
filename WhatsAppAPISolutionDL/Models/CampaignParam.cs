using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class CampaignParam
    {
        public int CampaignParamId { get; set; }
        public int? CampaignId { get; set; }
        public int? Sequence { get; set; }
        public string ParamName { get; set; }
        public string ParamText { get; set; }
        public int? ParamType { get; set; }
        public string ParamDefaultValue { get; set; }
        public bool? IsDynamic { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
