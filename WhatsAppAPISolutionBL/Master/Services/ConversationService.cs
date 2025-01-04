using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Conversation;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ConversationService : IConversationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<ConversationService> _logger;
        private readonly IHubContext<ConversationHub> _conversationHubContext;
        private readonly ICommunicationService _communicationService;

        public ConversationService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<ConversationService> logger,
            IHubContext<ConversationHub> conversationHubContext,
            ICommunicationService communicationService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _conversationHubContext = conversationHubContext;
            _communicationService = communicationService;
        }

        public async Task<List<UConversation>> GetConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, string conversationId = "",
            string waId = "", int moduleId = 0, int parentId = 0,
            int agentId = 0, int status = 0, string phoneNumber = "",
            string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.Conversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.List},@ClientId={clientId},@Id={id},@ConversationId={conversationId},@SenderId={senderId},@WaId={waId},@ModuleId={moduleId},@ParentId={parentId},@AgentId={agentId}, @Status={status}, @PhoneNumber={phoneNumber}, @SearchStr={searchStr},@SortBy={sortBy}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<List<UAgentConversationList>> GetAgentConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.AgentConversationLists.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AgentConversationList},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<List<UConversationListByConversation>> GetConversationListByConversationAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int messageId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.ConversationListByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationListByConversation},@ClientId={clientId},@Id={id}, @messageId={messageId},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddConversationToQueueAsync(int clientId = 0, int id = 0, string comment = "")
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AddConversationToQueue},@ClientId={clientId},@Id={id}, @Comment={comment}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int agentId = 0, string comment = "")
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.TransferConversationToAgent},@ClientId={clientId},@Id={id},@AgentId={agentId},@Comment={comment}").ToListAsync();
            if (response != null && response.Any())
            {
                // Look up the connection ID for the Agent ID and send the conversation
                string connectionId = String.Empty;
                int i;
                for (i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(agentId, out connectionId))
                    {
                        var conversations = await this.GetAgentConversationListAsync(clientId: clientId, agentId: agentId, id: id);
                        if (conversations != null && conversations.Any())
                        {
                            var conversation = conversations[0];
                            await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.ConversationAssigned.ToString(), conversation);
                            _logger.LogInformation("SignalR, triggered event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationAssigned.ToString(), agentId, id, i);
                            break;
                        }
                        else
                            _logger.LogError("SignalR, cannot find conversation with clientId {clienId}, agentId {agentId} and conversationId {conversationId} on try {try}", clientId, agentId, id, i);
                    }
                    else
                        _logger.LogError("SignalR, No connection found for event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationAssigned.ToString(), agentId, id, i);
                }

                //if (i >= 5) // If max retry exceeded, unassign the conversation again
                //    await this.AddConversationToQueueAsync(clientId: clientId, id: id, comment: "Cannot send the conversation to agent!");

                ////Send to all the agents except the agent that has been assigned just now
                //if (!String.IsNullOrEmpty(connectionId))
                //    await _conversationHubContext.Clients.AllExcept(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
                //else
                //    await _conversationHubContext.Clients.All.SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
            }

            return response[0];
        }

        /// <summary>
        /// This call will be call by assign agent background service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> models)
        {
            foreach (var item in models)
            {
                //If assigned agent template id is present, send a default template
                if (item.ActionId > 0)
                {
                    var template = await _dbContext.Templates.FindAsync(item.ActionId);
                    if (template != null && template.TransactionType == 2)
                    {
                        var bodyParams = new List<string> { item.AgentName };
                        await _communicationService.SendInteractiveMessageAsync(new UMessageReceived
                        {
                            ActionId = item.ActionId,
                            ModuleId = item.ModuleId,
                            ParentId = item.ParentId
                        }, template.ClientId ?? 0, item.PhoneNumber, "", bodyParams);
                    }
                }

                // Look up the connection ID for the Agent ID and send the conversation
                string connectionId = String.Empty;
                int i;
                for (i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(item.AgentId, out connectionId))
                    {
                        var conversations = await this.GetAgentConversationListAsync(clientId: item.ClientId, agentId: item.AgentId, id: item.ParentId);
                        if (conversations != null && conversations.Any())
                        {
                            var conversation = conversations[0];
                            await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.ConversationAssigned.ToString(), conversation);
                            _logger.LogInformation("SignalR, triggered event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationAssigned.ToString(), item.AgentId, item.ParentId, i);
                            break;
                        }
                        else
                            _logger.LogError("SignalR, cannot find conversation with clientId {clienId}, agentId {agentId} and conversationId {conversationId} on try {try}", item.ClientId, item.AgentId, item.ParentId, i);
                    }
                    else
                        _logger.LogError("SignalR, No connection found for event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationAssigned.ToString(), item.AgentId, item.ParentId, i);
                }

                //if (i >= 5) // If max retry exceeded, unassign the conversation again
                //    await this.AddConversationToQueueAsync(clientId: clientId, id: id, comment: "Cannot send the conversation to agent!");

                ////Send to all the agents except the agent that has been assigned just now
                //if (!String.IsNullOrEmpty(connectionId))
                //    await _conversationHubContext.Clients.AllExcept(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
                //else
                //    await _conversationHubContext.Clients.All.SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
            }

            return new UResponse
            {
                Status = 1,
                Message = "Data updated successfully"
            };
        }

        public async Task<ULatestConversationByConversation> GetLatestConversationMessageByConversationAsync(int clientId = 0, int id = 0)
        {
            var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetLatestConversationByConversationId},@ClientId={clientId},@Id={id}").ToListAsync();

            if (response.Any())
                return response[0];

            return null;
        }

        public async Task<List<UConversationReportList>> GetConversationReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.ConversationReports.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationReportList},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> ExpiredConversationNotifyToAgentAsync(List<ExpiredConversationDto> models)
        {
            foreach (var item in models)
            {
                //If assigned agent template id is present, send a default template
                if (item.ActionId > 0)
                {
                    var template = await _dbContext.Templates.FindAsync(item.ActionId);
                    if (template != null && template.TransactionType == 2)
                    {
                        var bodyParams = new List<string> { item.AgentName };
                        await _communicationService.SendInteractiveMessageAsync(new UMessageReceived
                        {
                            ActionId = item.ActionId,
                            ModuleId = item.ModuleId,
                            ParentId = item.ParentId
                        }, template.ClientId ?? 0, item.PhoneNumber, "", bodyParams);
                    }
                }

                // Look up the connection ID for the Agent ID and send the conversation
                string connectionId = String.Empty;
                int i;
                for (i = 1; i <= 5; i++)
                {
                    if (ConversationHub.connections.TryGetValue(item.AgentId, out connectionId))
                    {
                        var conversations = await this.GetAgentConversationListAsync(clientId: item.ClientId, agentId: item.AgentId, id: item.ParentId);
                        if (conversations != null && conversations.Any())
                        {
                            var conversation = conversations[0];
                            await _conversationHubContext.Clients.Client(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), conversation);
                            _logger.LogInformation("SignalR, triggered event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationUnAssigned.ToString(), item.AgentId, item.ParentId, i);
                            break;
                        }
                        else
                            _logger.LogError("SignalR, cannot find conversation with clientId {clienId}, agentId {agentId} and conversationId {conversationId} on try {try}", item.ClientId, item.AgentId, item.ParentId, i);
                    }
                    else
                        _logger.LogError("SignalR, No connection found for event {event} for agent id {agentId} with object {object} on try {try}", SignalREnum.ConversationUnAssigned.ToString(), item.AgentId, item.ParentId, i);
                }

                //if (i >= 5) // If max retry exceeded, unassign the conversation again
                //    await this.AddConversationToQueueAsync(clientId: clientId, id: id, comment: "Cannot send the conversation to agent!");

                ////Send to all the agents except the agent that has been assigned just now
                //if (!String.IsNullOrEmpty(connectionId))
                //    await _conversationHubContext.Clients.AllExcept(connectionId).SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
                //else
                //    await _conversationHubContext.Clients.All.SendAsync(SignalREnum.ConversationUnAssigned.ToString(), id);
            }

            return new UResponse
            {
                Status = 1,
                Message = "Data updated successfully"
            };
        }
    }
}
