using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Entity
{
    public class KfgPaymentRequest
    {
        public int MerchantId { get; set; }
        public string LicenceKey { get; set; }
        public string TransactionId { get; set; }
        public int MerchantTemplateId { get; set; }
        public string TransactionName { get; set; }
        public decimal Amount { get; set; }
        public string ReturnURL { get; set; }
        public string GatewayType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string? Currency { get; set; }
        public string? Language { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? UDF1 { get; set; }
        public string? UDF2 { get; set; }
        public string? UDF3 { get; set; }
    }

    public class KfgPaymentResponse
    {
        public int Code { get; set; }
        public bool success { get; set; }
        public string Message { get; set; }
        public string Result { get; set; } // Redirect URL
       // public string QrCode { get; set; }
    }

    public class KfgEncryptedResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Result { get; set; } // Encrypted base64 string
    }

    public class KfgDecryptedResponse
    {
        public string TID { get; set; }
        public string AMT { get; set; }
        public string GT { get; set; }
        public string RF { get; set; }
        public string ST { get; set; }
        public string RT { get; set; }
        public string? Ud1 { get; set; }
        public string? Ud2 { get; set; }
    }

 
    public class WebhookPayload
    {
        public string EncryptedKey { get; set; }
    }

    public class KfgDecryptedPayload
    {
        public string TID { get; set; }
        public string AMT { get; set; }
        public string GT { get; set; }
        public string RF { get; set; }
        public string ST { get; set; }
        public string RT { get; set; }
        public string Ud1 { get; set; }
        public string Ud2 { get; set; }
    }
    public class PaymentStatus
    {
        public bool IsSuccess { get; set; } 
        public string PaymentResult { get; set; }
        public string TransactionId { get; set; }
        public int OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
