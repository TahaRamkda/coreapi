namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UConversationListByConversation
    {
        public int? MessageId { get; set; }
        public int? ClientId { get; set; }
        public int? Id { get; set; }
        public int? AgentId { get; set; }
        public int? SenderId { get; set; }
        public int? ConversationId { get; set; }
        public int? TypeId { get; set; }
        public int? MessageTypeId { get; set; }
        public string MessageContent { get; set; }
        public int? Status { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string MediaPath { get; set; }
        public string CreatedDate { get; set; }
        public long? Line { get; set; }
    }
}
