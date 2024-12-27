namespace WhatsAppAPISolutionDL.Dto
{
    public partial class AgentDto
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AgentFName { get; set; }
        public string AgentLName { get; set; }
        public string PreferredLanguage { get; set; }
        public string SenderIds { get; set; }
        public int? ActionBy { get; set; }
    }
}
