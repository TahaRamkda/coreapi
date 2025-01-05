using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Agent
{
    public partial class UAgentSupervisorReport : UListEntity
    {
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public bool? IsDisabled { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; } 
        public int? TotalConversationsAssigned { get; set; }
        public int? TotalUnreadCount { get; set; }
    }
}
