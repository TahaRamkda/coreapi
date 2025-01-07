using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.Dto.Agent
{
    public class SendAgentInteractiveMessageRequestDto
    {
        public SendAgentInteractiveMessageRequestDto()
        {
            Values = new List<ParamValue>();
        }

        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int ConversationId { get; set; }
        public int InteractiveTemplateId { get; set; }
        public IFormFile File { get; set; }
        [JsonIgnore]
        public int MediaId { get; set; }
        public int ActionBy { get; set; }
        public List<ParamValue> Values { get; set; }
    }
}
