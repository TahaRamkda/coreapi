using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class WitAIService : IWitAIService
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IOptions<WitAiConfigurationSettings> _witAiConfigurationSettings;
        private readonly ILogger<WitAIService> _logger;

        #endregion

        #region Ctor

        public WitAIService(
          IHttpClientFactory httpClientFactory,
          IOptions<WitAiConfigurationSettings> witAiConfigurationSettings,
          ILogger<WitAIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.witai_api);
            _witAiConfigurationSettings = witAiConfigurationSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<string> SendContentToWitAi(string message)
        {
            string resultString = string.Empty;

            try
            {
                if (!_witAiConfigurationSettings.Value.Enabled)
                    return resultString;

                string apiEndpoint = $"/message?v={_witAiConfigurationSettings.Value.Version}&q={message}";
                var response = await _httpClient.GetAsync(apiEndpoint);
                _logger.LogInformation("WitAIResponseAsync - getting response from wit.ai response = {response}", JsonConvert.SerializeObject(response));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("WitAIResponseAsync - getting response from wit.ai success response = {response}", JsonConvert.SerializeObject(content));
                    var result = JsonConvert.DeserializeObject<WitAiResponseDto>(content);
                    if (result != null)
                    {
                        _logger.LogInformation("WitAIResponseAsync - getting response from wit.ai success DeserializeObject response = {response}", JsonConvert.SerializeObject(result));
                        if (result.Intents != null && result.Intents.Count > 0)
                        {
                            var intent = result.Intents[0];
                            resultString += $" - Intent: {intent.Name} ({intent.Confidence:F3})";
                        }
                        if (result.Traits?.WitSentiment != null && result.Traits.WitSentiment.Count > 0)
                        {
                            var sentiment = result.Traits.WitSentiment[0];
                            resultString += $" | Sentiment: {sentiment.Value} ({sentiment.Confidence:F3})";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return resultString;
        }

        #endregion
    }
}
