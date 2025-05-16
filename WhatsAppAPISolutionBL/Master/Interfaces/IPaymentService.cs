using Microsoft.Extensions.Options;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IPaymentService 
    {
       // Task<KfgPaymentResponse?> CreatePaymentAsync(KfgPaymentRequest request);
        Task<UResponse?> CheckKFGPaymentStatusAsync(string EncryptedString);
        Task<List<UResponse>> RecheckPaymentStatusAsync(List<long> orderIds);
        Task<KfgDecryptedResponse?> DecryptKfgResponse(string? EncryptedString);
        Task<UResponse?> TempCheckKFGPaymentStatusAsync(PaymentStatus paymentStatus);
    }
}
