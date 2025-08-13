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
    public class PaymentService : IPaymentService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<KFGPaymentConfiguration> _kfgpaymentConfigurationSettings;
        private readonly ILogger<PaymentService> _logger;
        public readonly IMediatorService _mediatorService;
        public readonly IAppSettingsService _appSettingsService;
        public readonly IOrderService _orderService;

        public PaymentService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            HttpClient httpClient,
            IOptions<KFGPaymentConfiguration> kfgpaymentConfigurationSettings,
            ILogger<PaymentService> logger, //,
            IMediatorService mediatorService,
            IAppSettingsService appSettingsService,
            IOrderService orderService
            )
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClient;
            _mediatorService = mediatorService;
            _kfgpaymentConfigurationSettings = kfgpaymentConfigurationSettings;
            _logger = logger;
            _appSettingsService = appSettingsService;
            _orderService = orderService;
        }

        public async Task<UResponse> CheckKFGPaymentStatusAsync(string encryptedString)
        {
            UResponse response = new UResponse();
            {
                response.Status = 1;
                response.Message = "OK";
            }

            PaymentStatus paymentStatus = new PaymentStatus();
            if (string.IsNullOrWhiteSpace(encryptedString))
            {
                response.Status = 0;
                response.Message = "No Encrypted String provided";
                return response;
            }

            var Decrypteddata = DecryptKfgResponse(encryptedString);
            if (Decrypteddata == null)
            {
                response.Status = 0;
                response.Message = "Error while decrypting Payment";
                return response;
            }
            else
            {
                paymentStatus.OrderId = Convert.ToInt32(Decrypteddata.TID);
                paymentStatus.PaymentRefNo = Decrypteddata.RF;
                paymentStatus.PaymentGatewayType = Decrypteddata.GT;
                if (!String.IsNullOrWhiteSpace(Decrypteddata.ST) && Decrypteddata.ST.Equals("CAPTURED", StringComparison.OrdinalIgnoreCase))
                    paymentStatus.IsSuccess = true;
                else
                    paymentStatus.IsSuccess = false;
            }

            var order = await _dbContext.Orders.FindAsync(paymentStatus.OrderId);
            if (order == null)
            {
                _logger.LogError("No order found with orderId={orderId} in Checkpaymentstatus in paymentService", paymentStatus.OrderId);
                response.Status = 0;
                response.Message = "No order found with provided response id";
                return response;
            }
 
            //For KFG, 1 = KNET and 2 = CREDIT CARD
            string paymentGateway = "credit_card";
            if (!String.IsNullOrWhiteSpace(paymentStatus.PaymentGatewayType) 
                && (paymentStatus.PaymentGatewayType == "1" || paymentStatus.PaymentGatewayType.ToLower() == "knet"))
                paymentGateway = "knet";

            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentResponse @OrderId={paymentStatus.OrderId},@Success={paymentStatus.IsSuccess},@PaymentRefNo={paymentStatus.PaymentRefNo},@PaymentGatewayType={paymentGateway}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PaymentCompleted with OrderId={OrderId} and TransactionId = {paymentStatus.IsSuccess}response={response}", paymentStatus.OrderId, paymentStatus.PaymentRefNo, JsonConvert.SerializeObject(dbresponse));

            await _mediatorService.ProcessDBResponse(order.ClientId ?? 0, order.SenderId ?? 0, dbresponse[0]);
             
            //Call order push service
            if (paymentStatus.IsSuccess)
            {
                //Reload order entity
                _dbContext.Entry<Order>(order).Reload();
                await _orderService.PushOrders(new List<int> { order.OrderId });
            }
                 
            return response;
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
                    // Fetch order from repository
                    var order = await _dbContext.Orders.FindAsync(Convert.ToInt32(orderId));

                    if (order == null)
                    {
                        _logger.LogError("Order not found for OrderId={OrderId}", orderId);
                        responses.Add(new UResponse
                        {
                            Message = $"Order with ID {orderId} not found.",
                            Status = 0
                        });
                        continue;
                    }

                    var _config = await _appSettingsService.GetAppSettingByKeyAsync(order.ClientId ?? 0, order.SenderId ?? 0, AppSettingKey.PaymentStatusUrl);
                    var clientintegration = await _appSettingsService.GetAppSettingByKeyAsync(order.ClientId ?? 0, order.SenderId ?? 0, AppSettingKey.ClientIntegrationType);

                    if (_config == null || String.IsNullOrWhiteSpace(_config.Val))
                    {
                        _logger.LogError("PaymentStatusUrl is not configured for clientId={clientId} and senderId={senderId}", order.ClientId ?? 0, order.SenderId ?? 0);
                        continue;
                    }

                    if (clientintegration == null || String.IsNullOrWhiteSpace(clientintegration.Val))
                    {
                        _logger.LogError("ClientIntegrationType is not configured for clientId={clientId} and senderId={senderId}", order.ClientId ?? 0, order.SenderId ?? 0);
                        continue;
                    }

                    var recheckUrl = _config.Val;
                    var integrationtype = Convert.ToInt32(clientintegration.Val);

                    if (integrationtype == (int)ClientIntegrationTypeEnum.KFG)
                    {
                        var requestBody = new
                        {
                            MerchantId = _kfgpaymentConfigurationSettings.Value.MerchantId,
                            LicenceKey = _kfgpaymentConfigurationSettings.Value.LicenseKey,
                            TransactionId = order.OrderId.ToString(),
                        };

                        var requestStr = JsonConvert.SerializeObject(requestBody);
                        var content = new StringContent(requestStr, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync(recheckUrl, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError("API call failed with status code {StatusCode} and reason {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                            responses.Add(new UResponse
                            {
                                Message = $"Error processing order ID {orderId}",
                                Status = 0
                            });
                        }

                        var responseStr = await response.Content.ReadAsStringAsync();

                        _logger.LogInformation("Received KFG payment status details for orderId={orderId} with request={request} and response={response}", orderId, requestStr, responseStr);

                        var paymenresponse = JsonConvert.DeserializeObject<RecheckKFGPaymentStatusRes>(responseStr);
                        if (paymenresponse == null)
                        {
                            _logger.LogError("Cannot parse KFG payment response for orderId={orderId} with request={request} and response={response}", orderId, requestStr, responseStr);
                            responses.Add(new UResponse
                            {
                                Status = 0,
                                Message = $"Cannot parse KFG payment status details for orderId={orderId}"
                            });
                        }
                        else
                        {
                            var paymentStatus = await CheckKFGPaymentStatusAsync(paymenresponse.result);

                            responses.Add(new UResponse
                            {

                                Message = paymentStatus.Message,
                                Status = paymentStatus.Status
                            });
                        }
                    }
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
            var dbresponse = await _dbContext2.DBResponses.FromSqlInterpolated($"exec usp_Orders_PaymentResponse @OrderId={paymentStatus.OrderId},@Success={paymentStatus.IsSuccess},@PaymentRefNo={paymentStatus.PaymentRefNo},@PaymentGatewayType={paymentStatus.PaymentGatewayType}").ToListAsync();
            _logger.LogInformation("Received response from procedure usp_Orders_PaymentCompleted with OrderId={OrderId} and TransactionId = {paymentStatus.IsSuccess}response={response}", paymentStatus.OrderId, paymentStatus.PaymentRefNo, JsonConvert.SerializeObject(dbresponse));


            await _mediatorService.ProcessDBResponse(order.ClientId ?? 0, order.SenderId ?? 0, dbresponse[0]);


            return response;
        }
    }
}
