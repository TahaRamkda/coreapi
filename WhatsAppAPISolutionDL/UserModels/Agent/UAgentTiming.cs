namespace WhatsAppAPISolutionDL.UserModels.Agent
{
    public class UAgentTiming
    {
        public int? ClientId { get; set; }
        public int? AgentId { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
