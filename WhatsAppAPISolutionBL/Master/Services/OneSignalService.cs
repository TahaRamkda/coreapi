using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.OneSignal;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class OneSignalService : IOneSignalService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<OneSignalConfigurationSettings> _oneSignalConfigurationSettings;

        public OneSignalService(
            IHttpClientFactory httpClientFactory,
            IOptions<OneSignalConfigurationSettings> oneSignalConfigurationSettings)
        {
            _httpClient = httpClientFactory.CreateClient(HttpClientType.one_signal_api);
            _oneSignalConfigurationSettings = oneSignalConfigurationSettings;
        }

        public async Task SendConversationAssignedNotification(UAgentConversationList conversation)
        {
            await SendNotification("Conversation Assigned", conversation.LastMessageText, conversation.AgentId.ToString());
        }

        public async Task SendMessageReceivedNotification(ULatestConversationByConversation conversation)
        {
            await SendNotification("Message Received", conversation.MessageContent, conversation.AgentId.ToString());
        }

        public async Task SendConversationUnAssignedNotification(int agentId)
        {
            await SendNotification("Conversation UnAssigned", "Conversation UnAssigned", agentId.ToString());
        }

        private async Task SendNotification(string headerContent, string bodyContent, string agentId)
        {
            var oneSignal = new OneSignalRequestDto()
            {
                app_id = _oneSignalConfigurationSettings.Value.AppId,
                include_external_user_ids = new List<string> { agentId },
                contents = new Dictionary<string, string> { { "en", bodyContent } },
                headings = new Dictionary<string, string> { { "en", headerContent } }
            };
            var request = JsonConvert.SerializeObject(oneSignal);
            var res = new StringContent(request, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", _oneSignalConfigurationSettings.Value.Token);
            var response = await _httpClient.PostAsync($"/api/v1/notifications", res);
            var content = await response.Content.ReadAsStringAsync();
        }
    }
}
