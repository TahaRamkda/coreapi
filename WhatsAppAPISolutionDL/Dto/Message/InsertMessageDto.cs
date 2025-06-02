using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionDL.Dto.Message
{
    public partial class InsertMessageDto
    {
        public InsertMessageDto()
        {
            Conversation = new ConversationDto();
            Pricing = new PricingDto();
            Error = new ErrorDto();
        }

        public int ParentId { get; set; }
        public int ModuleId { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int MessageReferenceId { get; set; }
        public string WaId { get; set; }
        public int MediaId { get; set; }
        public MessageStatusEnum Status { get; set; }
        public string UpdateDateTime { get; set; }
        public string RecipientId { get; set; }
        public string MessageContent { get; set; }
        public int MessageType { get; set; }
        public string ButtonJson { get; set; }
        public bool SystemGenerated { get; set; }
        public ConversationDto Conversation { get; set; }
        public PricingDto Pricing { get; set; }
        public ErrorDto Error { get; set; }

        public PhoneNumberDto PhoneNumberId { get; set; }

        public class PhoneNumberDto
        {
            public string DisplayPhoneNumber { get; set; }
            public string PhoneNumberId { get; set; }
        }

        public class ConversationDto
        {
            public string Id { get; set; }
            public string OriginType { get; set; }
        }

        public class PricingDto
        {
            public bool Billable { get; set; }
            public string PricingModel { get; set; }
            public string Category { get; set; }
        }

        public class ErrorDto
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string ErrorDetails { get; set; }
        }
    }
}
