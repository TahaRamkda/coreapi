namespace WhatsAppAPISolutionDL.Dto.Agent
{
    public class AgentTimingDto
    {
        public AgentTimingDto()
        {
            Timings = new List<AgentTiming>();
        }

        //public int ClientId { get; set; }
        public int AgentId { get; set; }
        //public int ActionBy { get; set; }

        public List<AgentTiming> Timings { get; set; }

        public class AgentTiming
        {
            public int WeekDay { get; set; }
            public string WeekDayName { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
        }
    }
}
