namespace WhatsAppAPISolutionDL.Dto.Campaign
{
    public partial class CampaignDto
    {
        public CampaignDto()
        {
            CampaignParameters = new List<CampaignParamDto>();
            CampaignContacts = new List<CampaignContactDto>();
        }

        //public int ClientId { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int SenderId { get; set; }
        public int TemplateId { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string CampaignType { get; set; }
        public string GroupIds { get; set; }
        public int MediaId { get; set; }
        //public int? ActionBy { get; set; }
        public List<CampaignParamDto> CampaignParameters { get; set; }
        public List<CampaignContactDto> CampaignContacts { get; set; }
    }

    public partial class CampaignParamDto
    {
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public int? ParamType { get; set; } 
        public int? Sequence { get; set; }
    }

    public partial class CampaignContactDto
    {
        public int? GroupId { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Area { get; set; }
        public decimal Cost { get; set; }
        public string SendStatus { get; set; }
    }
}
