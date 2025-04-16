namespace WhatsAppAPISolutionDL.Dto.ConversationAnalytic
{
    public class ConversationAnalyticRequestDto
    {
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
