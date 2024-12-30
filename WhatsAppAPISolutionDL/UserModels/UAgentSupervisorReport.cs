namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UAgentSupervisorReport
    {
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public bool? IsDisabled { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? TotalRecords { get; set; }
        public int? TotalConversationsAssigned { get; set; }
        public int? TotalUnreadCount { get; set; }
        public long? Line { get; set; }
        public int? Total { get; set; }
    }
}
