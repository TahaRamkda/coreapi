using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class KFGPaymentService: IKFGPaymentService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "";
        private readonly string _merchantId = "";
        private readonly string _licenseKey = "";
        private readonly string _secretKey = "";
        private readonly KFGPaymentConfiguration _config;
        //private readonly IMessageService _messageService;
        private readonly ILogger<KFGPaymentService> _logger;
        public readonly IMediatorService _mediatorService;

        public KFGPaymentService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient, 
            IOptions<KFGPaymentConfiguration> config,
            ILogger<KFGPaymentService> logger, //,
            IMediatorService mediatorService
            //IMessageService messageService

            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _config = config.Value;
            _mediatorService = mediatorService;
            _baseUrl=_config.BaseURL;
            _merchantId=_config.MerchantId;
            _licenseKey=_config.LicenseKey;
            _secretKey=_config.SecretKey;
            _logger = logger;
            //_messageService = messageService;
        }

      

        public async Task<UResponse?> CheckPaymentStatusAsync(PaymentStatus paymentStatus)
        {
            UResponse response = new UResponse();
            {
                response.Status = 0;
                response.Message= "OK";
            }
            var order = await _dbContext.Orders.FindAsync(paymentStatus.OrderId);
            if (order == null)
            {
                _logger.LogError("No order found with orderId={orderId} in Checkpaymentstatus in paymentService", paymentStatus.OrderId);
                response.Status = 1;
                response.Message = "No order found with provided response id";
                return response;
            }
            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentCompleted @OrderId={paymentStatus.OrderId},@Success={paymentStatus.IsSuccess},@TransactionId={paymentStatus.TransactionId}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PaymentCompleted with OrderId={OrderId} and TransactionId = {paymentStatus.IsSuccess}response={response}",paymentStatus.OrderId, paymentStatus.TransactionId, JsonConvert.SerializeObject(dbresponse));

          await _mediatorService.ProcessDBResponse(order.ClientId ?? 0, order.SenderId ?? 0, dbresponse[0]);

            //var payload = new
            //{
            //    MerchantId = int.Parse(_merchantId),
            //    LicenceKey = _licenseKey,
            //    TransactionId = paymentStatus.TransactionId,
            //};

            //var url = $"{_baseUrl}/GetPaymentInformation";
            //var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();

            //var json = await response.Content.ReadAsStringAsync();
            //var result = JsonSerializer.Deserialize<KfgEncryptedResponse>(json);

            return response ;
        }

        public async Task<KfgDecryptedResponse?> DecryptKfgResponse(string? encryptedBase64)
        {
            if (string.IsNullOrWhiteSpace(encryptedBase64)) return null;

            byte[] keyBytes = Encoding.UTF8.GetBytes(_secretKey);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            var decryptedJson = Encoding.UTF8.GetString(decryptedBytes);

            return JsonConvert.DeserializeObject<KfgDecryptedResponse>(decryptedJson);
        }
    }
}
