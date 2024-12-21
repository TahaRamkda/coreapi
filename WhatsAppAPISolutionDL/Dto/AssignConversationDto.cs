namespace WhatsAppAPISolutionDL.Dto
{
    public class AssignConversationDto
    {
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public int AgentId { get; set; }
        public int AssignedAgentTemplateId { get; set; }
        public string AgentName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
