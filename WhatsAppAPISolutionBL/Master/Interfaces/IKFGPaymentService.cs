using Microsoft.Extensions.Options;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IKFGPaymentService
    {
        Task<KfgPaymentResponse?> CreatePaymentAsync(KfgPaymentRequest request);
        Task<KfgDecryptedResponse?> CheckPaymentStatusAsync(string transactionId);
        Task<KfgDecryptedResponse?> DecryptKfgResponse(string? encryptedBase64);
    }
}
