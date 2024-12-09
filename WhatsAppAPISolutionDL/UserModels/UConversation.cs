namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UConversation
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string ConversationId { get; set; }
        public string WaId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public int? ConversationType { get; set; }
        public string ConversationMode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Commission { get; set; }
        public int? AgentId { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string ExpiryDate { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public long? Line { get; set; }
        public int? TotalItems { get; set; }
    }
}
