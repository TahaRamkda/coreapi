namespace WhatsAppAPISolutionDL.UserModels
{
    public class UAgentDetail
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public string AgentFName { get; set; }
        public string AgentLName { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public bool? IsDisabled { get; set; }
        public string PreferredLanguage { get; set; }
        public string LastOnline { get; set; }
        public string CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string SenderIds { get; set; } 
    }
}
