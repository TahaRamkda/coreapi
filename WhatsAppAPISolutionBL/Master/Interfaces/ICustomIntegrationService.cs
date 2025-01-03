using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ICustomIntegrationService
    {
        public Task<ApiResult> SendSmsAsync(SendSmsDto sendSms, int ClientId, int UserId);
    }
}
