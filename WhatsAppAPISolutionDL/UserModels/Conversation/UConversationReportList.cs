using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Conversation
{
    public class UConversationReportList : UListEntity
    {
        public int? Id { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public int? TotalMessages  { get; set; }
        public string ConversationId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string UpdatedDate { get; set; }
        public int? LastMessageId { get; set; }
        public string LastMessageText { get; set; }
        public int? LastMessageTypeId { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public string Logo { get; set; }
        public int? UnreadCount { get; set; }
        public string CreatedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string ReasonName { get; set; }
        public int? ReasonId { get; set; }
    }
}
