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
        public string ParamValue { get; set; }
        public int? ParamType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
