using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICustomIntegrationService
    {
        public Task<UResponse> SendSmsAsync(SendSmsDto sendSms, int ClientId, int UserId);
    }
}
