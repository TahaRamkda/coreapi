using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.OneSignal;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OneSignalService : IOneSignalService
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IOptions<OneSignalConfigurationSettings> _oneSignalConfigurationSettings;
        private readonly ILogger<OneSignalService> _logger;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        #endregion

        #region Ctor

        public OneSignalService(
            IHttpClientFactory httpClientFactory,
            IOptions<OneSignalConfigurationSettings> oneSignalConfigurationSettings,
            ILogger<OneSignalService> logger,
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.one_signal_api);
            _oneSignalConfigurationSettings = oneSignalConfigurationSettings;
            _logger = logger;
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        #endregion

        #region Utilities

        private async Task SendNotification(string headerContent, string bodyContent, string agentId, string language = "en")
        {
            try
            {
                if (!_oneSignalConfigurationSettings.Value.Enabled)
                    return;

                _logger.LogDebug("Calling One Signal api SendNotification with headerContent={headerContent}, bodyContent={bodyContent}, agentId={agentId}, language={language}", headerContent, bodyContent, agentId, language);

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
                var response = await _httpClient.PostAsync(apiEndpoint, res);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response from One Signal API apiEndpoint={apiEndpoint} notifications with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred {exception} when executing function One Signal SendNotification with headerContent={headerContent} bodyContent={bodyContent} agentId={agentId} language={language}", ex, headerContent, bodyContent, agentId, language);
            }
        }

        #endregion

        #region Methods

        public async Task<bool> IsAgentOneSignalEnabled(int? clientId, int? senderId = 0)
        {
            _logger.LogDebug("Calling api IsAgentOneSignalEnabled with clientId={clientId}, senderId={senderId}", clientId, senderId);
            string keyNames = CommonEnum.IsOneSignalEnabled.ToString();
            var response = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={senderId}").ToListAsync();
            _logger.LogDebug("Recieved api IsAgentOneSignalEnabled response with response={response}", JsonConvert.SerializeObject(response));
            if (!response.Any()) return false;
            else return response[0].Val == "0" ? false : true;
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

        #endregion
    }
}
