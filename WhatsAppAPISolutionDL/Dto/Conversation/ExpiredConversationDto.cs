namespace WhatsAppAPISolutionDL.Dto.Conversation
{
    public class ExpiredConversationDto
    {
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public int ParentId { get; set; }
        public int ModuleId { get; set; }
        public int ActionId { get; set; }
    }
}
