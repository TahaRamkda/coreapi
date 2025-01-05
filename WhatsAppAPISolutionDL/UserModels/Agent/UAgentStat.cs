namespace WhatsAppAPISolutionDL.UserModels.Agent
{
    public class UAgentStat
    {
        public int TotalAssigned { get; set; }
        public int TotalActive { get; set; }
        public int TotalClosed { get; set; }
        public int ExpiredChats { get; set; }
        public int ForceClosedChats { get; set; }
        public int ClosedChats { get; set; }
        public int AvgChatDuration { get; set; }
        public int ResponseTime { get; set; }
    }
}
