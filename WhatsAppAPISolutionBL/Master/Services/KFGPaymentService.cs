using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using WhatsAppAPISolutionDL.UserModels.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class KFGPaymentService
    {     
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "";
        private readonly string _merchantId = "";
        private readonly string _licenseKey = "";
        private readonly string _secretKey = "";
        private readonly KFGPaymentConfiguration _config;

        public KFGPaymentService(HttpClient httpClient, IOptions<KFGPaymentConfiguration> config)
        {
            _httpClient = httpClient;
            _config = config.Value;

            _baseUrl=_config.BaseURL;
            _merchantId=_config.MerchantId;
            _licenseKey=_config.LicenseKey;
            _secretKey=_config.SecretKey;
        }

        public async Task<KfgPaymentResponse?> CreatePaymentAsync(KfgPaymentRequest request)
        {
            request.MerchantId = int.Parse(_merchantId);
            request.LicenceKey = _licenseKey;

            var url = $"{_baseUrl}/CreatePaymentRequest";
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<KfgPaymentResponse>(json);
        }

        public async Task<KfgDecryptedResponse?> CheckPaymentStatusAsync(string transactionId)
        {
            var payload = new
            {
                MerchantId = int.Parse(_merchantId),
                LicenceKey = _licenseKey,
                TransactionId = transactionId
            };

            var url = $"{_baseUrl}/GetPaymentInformation";
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<KfgEncryptedResponse>(json);

            return await DecryptKfgResponse(result?.Result);
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

            return JsonSerializer.Deserialize<KfgDecryptedResponse>(decryptedJson);
        }     
    }
}
