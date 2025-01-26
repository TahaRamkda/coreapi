using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WhatsAppAPISolutionDL.UserModels.Campaign
{
    public class UCampaignDetail
    {
        public UCampaignDetail()
        {
            Parameters = new List<Parameter>();
        }

        public int? CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public int? TemplateId { get; set; }
        public string ScheduleDate { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? TotalContacts { get; set; }
        public int? SentCount { get; set; }
        public int? FailedCount { get; set; }
        public int? DeliveredCount { get; set; }
        public int? UndeliveredCount { get; set; }
        public string GroupIds { get; set; }
        public int? MediaId { get; set; }
        public string MediaURL { get; set; }
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedDate { get; set; }

        [JsonIgnore]
        public string ParamsJson { get; set; }

        [NotMapped] 
        public List<Parameter> Parameters { get; set; }

        public class Parameter
        {
            public int? CampaignParamId { get; set; } 
            public int? Sequence { get; set; }
            public string ParamName { get; set; }
            public string ParamValue { get; set; }
            public int? ParamType { get; set; }
        }
    }
}
