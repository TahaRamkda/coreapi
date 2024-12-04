using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICommunicationService
    {
        Task<UResponse> SendTemplateMessageAsync(TemplateMessagePayloadDto templateMessage);
        Task<UResponse> SendMessageAsync(SendMessageRequestDto model);
        Task<UResponse> SendInteractiveMessageAsync(UMessageReceived model, int clientId, string phoneNumber);
    }
}
