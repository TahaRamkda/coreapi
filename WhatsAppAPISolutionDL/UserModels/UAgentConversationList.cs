namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UAgentConversationList
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string ConversationId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string UpdatedDate { get; set; }
        public string LastMessage { get; set; }
    }
}
