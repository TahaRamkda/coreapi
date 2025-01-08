namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class ULatestConversationByConversation
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
        public string Logo { get; set; }
        public int? ParentMessageId { get; set; }
        public int? ParentMessageTypeId { get; set; }
        public string ParentMessageContent { get; set; }
        public int? ParentMediaId { get; set; }
        public string ButtonJson { get; set; }
        public string CreatedDate { get; set; }
    }
}
