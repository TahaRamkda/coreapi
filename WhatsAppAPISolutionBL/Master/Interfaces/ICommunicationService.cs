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
        Task<ApiResult> SendCarouselTemplateMessageAsync(TemplateMessagePayloadDto model);
        Task<ApiResult> SendMessageAsync(SendMessageRequestDto model);
        Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, int senderId, string phoneNumber, int mediaId = 0, List<ParamValue> values = null, string flowToken = "");
        Task<ApiResult> SendAgentMessageAsync(SendAgentMessageRequestDto model);
        Task<ApiResult> SendAgentInteractiveMessageAsync(SendAgentInteractiveMessageRequestDto model);
        Task<ApiResult> SendInteractiveMessageAsync(InteractiveMessageRequestDto model);
        Task<ApiResult> SendInteractiveTemplateMessageAsync(TemplateMessagePayloadDto model);
    }
}
