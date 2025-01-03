using Microsoft.AspNetCore.Http;

namespace WhatsAppAPISolutionDL.Dto.Agent
{
    public partial class SendAgentMessageRequestDto
    {
        public SendAgentMessageRequestDto()
        {

        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public int MediaId { get; set; }
        public string Message { get; set; }
        public IFormFile File { get; set; }
        public int ActionBy { get; set; }
    }
}
