namespace WhatsAppAPISolutionDL.UserModels
{
    public class UCampaignDetailParam
    {
        public int? CampaignParamId { get; set; }
        public int? CampaignId { get; set; }
        public int? Sequence { get; set; }
        public string ParamName { get; set; }
        public string ParamText { get; set; }
        public int? ParamType { get; set; }
        public string ParamDefaultValue { get; set; }
        public bool? IsDynamic { get; set; }
        public int? Status { get; set; } 
    }
}
