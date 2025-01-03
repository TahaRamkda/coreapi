using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICommunicationService
    {
        Task<ApiResult> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage);
        Task<UResponse> SendMessageAsync(SendMessageRequestDto model);
        Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, string phoneNumber, string headerParam = "", List<string> bodyParam = null);
        Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model);
    }
}
