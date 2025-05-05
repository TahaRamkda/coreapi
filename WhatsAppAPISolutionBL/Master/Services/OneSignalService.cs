using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.OneSignal;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OneSignalService : IOneSignalService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<OneSignalConfigurationSettings> _oneSignalConfigurationSettings;
        private readonly ILogger<OneSignalService> _logger;

        public OneSignalService(
            IHttpClientFactory httpClientFactory,
            IOptions<OneSignalConfigurationSettings> oneSignalConfigurationSettings,
            ILogger<OneSignalService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.one_signal_api);
            _oneSignalConfigurationSettings = oneSignalConfigurationSettings;
            _logger = logger;
        }

        public async Task SendConversationAssignedNotification(int agentId, string language, string message)
        {
            _logger.LogDebug("Calling api SendConversationAssignedNotification with agentId={agentId} language={language} and message={message}", agentId, language, message);
            await SendNotification("Conversation Assigned", message, agentId.ToString(), language);
        }

        public async Task SendMessageReceivedNotification(int agentId, string language, string message)
        {
            _logger.LogDebug("Calling api SendMessageReceivedNotification with agentId={agentId} language={language} and message={message}", agentId, language, message);
            await SendNotification("Message Received", message, agentId.ToString(), language);
        }

        public async Task SendConversationUnAssignedNotification(int agentId)
        {
            _logger.LogDebug("Calling api SendConversationUnAssignedNotification with agentId={agentId}", agentId);
            await SendNotification("Conversation Unassigned", "Conversation Unassigned", agentId.ToString());
        }

        private async Task SendNotification(string headerContent, string bodyContent, string agentId, string language = "en")
        {
            _logger.LogDebug("Calling api SendNotification with headerContent={headerContent}, bodyContent={bodyContent}, agentId={agentId}, language={language}", headerContent, bodyContent, agentId, language);
            
            var oneSignal = new OneSignalRequestDto()
            {
                app_id = _oneSignalConfigurationSettings.Value.AppId,
                include_external_user_ids = new List<string> { agentId },
                contents = new Dictionary<string, string> { { language.ToLower(), bodyContent } },
                headings = new Dictionary<string, string> { { language.ToLower(), headerContent } }
            };

            var request = JsonConvert.SerializeObject(oneSignal);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = $"/api/v1/notifications";

            var res = new StringContent(request, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _oneSignalConfigurationSettings.Value.Token);
            var response = await _httpClient.PostAsync(apiEndpoint, res);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Calling One Signal API apiEndpoint={apiEndpoint} notifications with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);
            _logger.LogDebug("Recieved api SendNotification response with response={response}", content);
        }
    }
}
