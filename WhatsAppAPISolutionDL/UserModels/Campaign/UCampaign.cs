using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Campaign
{
    public partial class UCampaign : UListEntity
    {
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public string ScheduleDate { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string GroupIds { get; set; }
        public int? TotalContacts { get; set; }
        public int? SentCount { get; set; }
        public int? ReadCount { get; set; }
        public int? FailedCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? UndeliveredCount { get; set; }
        public string CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
