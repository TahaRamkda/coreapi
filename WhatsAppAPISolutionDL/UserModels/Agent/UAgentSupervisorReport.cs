using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Agent
{
    public partial class UAgentSupervisorReport : UListEntity
    {
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public int? ActiveChat { get; set; }
        public int? AssignedChat { get; set; }
        public int? UnAssignedChat { get; set; }
        public int? AbandonChat { get; set; }
        public int? ExpiredChat { get; set; }
        public int? ForceClosedChat { get; set; }
        public int? ClosedChat { get; set; }
        public decimal? Rating { get; set; }
        public int? AvgResponseTime { get; set; }
        public int? AvgChatTime { get; set; }
        public int? IsDisabled { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }   
    }
}
