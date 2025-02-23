using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Agent
{
    public partial class UAgent : UListEntity
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
        public string SenderIds { get; set; }
        public string ChatReasonIds { get; set; }
        public string AgentFNameAR { get; set; }
        public string AgentLNameAR { get; set; }
    }
}
