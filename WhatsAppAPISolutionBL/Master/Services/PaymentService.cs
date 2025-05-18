using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Order.KFG.Payments;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Location;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class PaymentService: IPaymentService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<KFGPaymentConfiguration> _kfgpaymentConfigurationSettings;
        private readonly ILogger<PaymentService> _logger;
        public readonly IMediatorService _mediatorService;
        public readonly IAppSettingsService _appSettingsService;

        public PaymentService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient,
            IOptions<KFGPaymentConfiguration> kfgpaymentConfigurationSettings,
            ILogger<PaymentService> logger, //,
            IMediatorService mediatorService,
            IAppSettingsService appSettingsService
            //IMessageService messageService

            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _mediatorService = mediatorService;
           _kfgpaymentConfigurationSettings = kfgpaymentConfigurationSettings;
            _logger = logger;
            _appSettingsService = appSettingsService;
            //_messageService = messageService;
        }

        public async Task<UResponse> CheckKFGPaymentStatusAsync(string encryptedString)
        {
            UResponse response = new UResponse();
            {
                response.Status = 1;
                response.Message= "OK";
            }
            PaymentStatus paymentStatus = new PaymentStatus();
            if (string.IsNullOrWhiteSpace(encryptedString))
            {
                response.Status = 0;
                response.Message = "No Encrypted String provided";
                return response;
            }
            var Decrypteddata =  DecryptKfgResponse(encryptedString);
            if(Decrypteddata == null)
            {
                response.Status = 0;
                response.Message = "Error while decrypting Payment";
                return response;

            }
            else
            {
                paymentStatus.OrderId = Convert.ToInt32(Decrypteddata.TID);
                paymentStatus.TransactionId = Decrypteddata.RF;
                if (Decrypteddata.ST.ToUpper() !="CAPTURED")
                {
                    paymentStatus.IsSuccess = false;
                }
                else
                {
                    paymentStatus.IsSuccess = true;
                }
            }
            var order = await _dbContext.Orders.FindAsync(paymentStatus.OrderId);
            if (order == null)
            {
                _logger.LogError("No order found with orderId={orderId} in Checkpaymentstatus in paymentService", paymentStatus.OrderId);
                response.Status = 0;
                response.Message = "No order found with provided response id";
                return response;
            }
            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentCompleted @OrderId={paymentStatus.OrderId},@Success={paymentStatus.IsSuccess},@TransactionId={paymentStatus.TransactionId}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PaymentCompleted with OrderId={OrderId} and TransactionId = {paymentStatus.IsSuccess}response={response}",paymentStatus.OrderId, paymentStatus.TransactionId, JsonConvert.SerializeObject(dbresponse));

          await _mediatorService.ProcessDBResponse(order.ClientId ?? 0, order.SenderId ?? 0, dbresponse[0]);


            return response ;
        }
        public async Task<List<UResponse>> RecheckPaymentStatusAsync(List<long> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
            {
               // _logger.LogWarning("CheckKFGPaymentStatusAsync called with null or empty orderIds.");
                return new List<UResponse>
                {
                    new UResponse
                    {
                        Message = "No order IDs provided.",
                        Status = 0
                    }
                };
            }

            var responses = new List<UResponse>();

            foreach (var orderId in orderIds)
            {
                try
                {
                    _logger.LogInformation("Fetching order details for OrderId={OrderId}", orderId);

                    // Fetch order from repository
                    var order = await _dbContext.Orders.FindAsync(Convert.ToInt32(orderId));

                    if (order == null)
                    {
                        _logger.LogWarning("Order not found for OrderId={OrderId}", orderId);
                        responses.Add(new UResponse
                        {
                            Message = $"Order with ID {orderId} not found.",
                            Status = 0
                        });
                        continue;
                    }
                    
                    var _config = await _appSettingsService.GetAppSettingByKeyAsync( order.ClientId ?? 0,  order.SenderId ?? 0,AppSettingKey.PaymentStatusUrl);
                    var recheckUrl = _config.Val;
                    var requestBody = new 
                    {
                        MerchantId =_kfgpaymentConfigurationSettings.Value.MerchantId,
                        LicenceKey = _kfgpaymentConfigurationSettings.Value.LicenseKey,
                        TransactionId = order.OrderId.ToString(),
                    };
                    var jsonBody = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(recheckUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("API call failed with status code {StatusCode} and reason {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                        responses.Add(new UResponse
                        {
                            Message = $"Error processing order ID {orderId}",
                            Status = 0
                        });
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymenresponse = JsonConvert.DeserializeObject<RecheckKFGPaymentStatusRes>(responseContent);
                    var paymentStatus = await CheckKFGPaymentStatusAsync(paymenresponse.result);
                   
                    responses.Add(new UResponse
                    {

                        Message =paymentStatus.Message,
                        Status = paymentStatus.Status
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing OrderId={OrderId}", orderId);
                    responses.Add(new UResponse
                    {
                       
                        Message = $"Error processing order ID {orderId}: {ex.Message}",
                        Status = 1
                    });
                }
            }

            return responses;
        }
        //public async Task<KfgDecryptedResponse?> DecryptKfgResponse(string EncryptedString)
        //{
        //    if (string.IsNullOrWhiteSpace(EncryptedString)) return null;

        //    string rawKey = _kfgpaymentConfigurationSettings.Value.SecretKey; // "asx687@qw"
        //    string paddedKey = rawKey.PadRight(16, '0'); // Pads it to 16 characters: "asx687@qw0000000"
        //    byte[] keyBytes = Encoding.UTF8.GetBytes(paddedKey);

        //    byte[] encryptedBytes = Convert.FromBase64String(EncryptedString);

        //    using var aes = Aes.Create();
        //    aes.Key = keyBytes;
        //    aes.Mode = CipherMode.ECB;
        //    aes.Padding = PaddingMode.PKCS7;

        //    using var decryptor = aes.CreateDecryptor();
        //    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        //    var decryptedJson = Encoding.UTF8.GetString(decryptedBytes);

        //    return JsonConvert.DeserializeObject<KfgDecryptedResponse>(decryptedJson);
        //}

        public KfgDecryptedResponse DecryptKfgResponse(string EncryptedString)
        {
            EncryptedString = EncryptedString.Replace(" ", "+");
            string rawKey = _kfgpaymentConfigurationSettings.Value.SecretKey; // "asx687@qw"
            byte[] cipherBytes = Convert.FromBase64String(EncryptedString);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(rawKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                encryptor.Key = pdb.GetBytes(32);

                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);

                        cs.Close();

                    }
                    EncryptedString = Encoding.Unicode.GetString(ms.ToArray());

                }
            }
            return JsonConvert.DeserializeObject<KfgDecryptedResponse>(EncryptedString);

        }
        public async Task<UResponse?> TempCheckKFGPaymentStatusAsync(PaymentStatus paymentStatus)
        {
            UResponse response = new UResponse();
            {
                response.Status = 1;
                response.Message = "OK";
            }
          
            var order = await _dbContext.Orders.FindAsync(paymentStatus.OrderId);
            if (order == null)
            {
                _logger.LogError("No order found with orderId={orderId} in Checkpaymentstatus in paymentService", paymentStatus.OrderId);
                response.Status = 0;
                response.Message = "No order found with provided response id";
                return response;
            }
            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentCompleted @OrderId={paymentStatus.OrderId},@Success={paymentStatus.IsSuccess},@TransactionId={paymentStatus.TransactionId}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PaymentCompleted with OrderId={OrderId} and TransactionId = {paymentStatus.IsSuccess}response={response}", paymentStatus.OrderId, paymentStatus.TransactionId, JsonConvert.SerializeObject(dbresponse));

            await _mediatorService.ProcessDBResponse(order.ClientId ?? 0, order.SenderId ?? 0, dbresponse[0]);


            return response;
        }
    }
}
