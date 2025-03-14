using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.OneSignal;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;

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

        public async Task SendConversationAssignedNotification(UAgentConversationList conversation)
        {
            _logger.LogInformation("Calling api SendConversationAssignedNotification with request={request}", conversation);
            await SendNotification("Conversation Assigned", conversation.LastMessageText, conversation.AgentId.ToString(), conversation.Language);
        }

        public async Task SendMessageReceivedNotification(ULatestConversationByConversation conversation)
        {
            _logger.LogInformation("Calling api SendMessageReceivedNotification with request={request}", conversation);
            await SendNotification("Message Received", conversation.MessageContent, conversation.AgentId.ToString(), conversation.Language);
        }

        public async Task SendConversationUnAssignedNotification(int agentId)
        {
            _logger.LogInformation("Calling api SendConversationUnAssignedNotification with agentId={agentId}", agentId);
            await SendNotification("Conversation Unassigned", "Conversation Unassigned", agentId.ToString());
        }

        private async Task SendNotification(string headerContent, string bodyContent, string agentId, string language = "en")
        {
            _logger.LogInformation("Calling api SendNotification with headerContent={headerContent}, bodyContent={bodyContent}, agentId={agentId}, language={language}", headerContent, bodyContent, agentId, language);
            var oneSignal = new OneSignalRequestDto()
            {
                app_id = _oneSignalConfigurationSettings.Value.AppId,
                include_external_user_ids = new List<string> { agentId },
                contents = new Dictionary<string, string> { { language.ToLower(), bodyContent } },
                headings = new Dictionary<string, string> { { language.ToLower(), headerContent } }
            };
            var request = JsonConvert.SerializeObject(oneSignal);
            var res = new StringContent(request, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", _oneSignalConfigurationSettings.Value.Token);
            var response = await _httpClient.PostAsync($"/api/v1/notifications", res);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Recieved api SendNotification response with response={response}", JsonConvert.SerializeObject(content));
        }
    }
}
