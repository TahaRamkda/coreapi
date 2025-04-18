namespace WhatsAppAPISolutionDL.Dto.ConversationAnalytic
{
    public class ConversationAnalyticBridgeRequestDto
    {
        public string ClientId { get; set; }
        public string SenderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
