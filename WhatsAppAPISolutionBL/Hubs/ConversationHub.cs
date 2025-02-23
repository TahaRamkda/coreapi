using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionDL.Hubs
{
    public class ConversationHub : Hub
    {
        public static readonly Dictionary<int, string> connections = new();
        private readonly IAgentsService _agentsService;
        private readonly ILogger<ConversationHub> _logger;

        public ConversationHub(IAgentsService agentsService,
            ILogger<ConversationHub> logger)
        {
            _agentsService = agentsService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // Retrieve the Agent ID from the query string
            var agentId = Context.GetHttpContext()?.Request.Query["AgentId"];

            if (int.TryParse(agentId, out int parsedAgentId))
            {
                string connectionId = String.Empty;
                lock (connections)
                {
                    //If connection already exists, then remove
                    if (connections.TryGetValue(parsedAgentId, out connectionId))
                        connections.Remove(parsedAgentId);

                    connections[parsedAgentId] = Context.ConnectionId;
                    connectionId = Context.ConnectionId;
                }

                _logger.LogInformation("Agent {agentId} connected on ConversationHub with connectionId {connectionId}", parsedAgentId, connectionId);

                await Clients.Client(connectionId).SendAsync(SignalREnum.Connected.ToString(), $"Agent connected with agent id - {agentId} and connection id - {connectionId}");

                //Make agent active
                //await _agentsService.SetAgentStatusAsync(Convert.ToInt32(parsedAgentId), (int)AgentStatusEnum.Active);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove the Agent's connection ID when disconnected
            var connectionId = Context.ConnectionId;
            int agentId = 0;
            lock (connections)
            {
                agentId = connections.FirstOrDefault(x => x.Value == connectionId).Key;
                if (agentId != 0)
                {
                    connections.Remove(agentId);
                }
            }

            _logger.LogInformation("Agent {agentId} disconnected on ConversationHub with connectionId {connectionId}", agentId, connectionId);

            await Clients.Client(connectionId).SendAsync(SignalREnum.DisConnected.ToString(), $"Agent disconnected with agent id - {agentId} and connection id - {connectionId}");

            // Optionally handle the exception if provided
            if (exception != null)
                _logger.LogError("Exception occurred, Agent {agentId} disconnected on ConversationHub with connectionId {connectionId} and with exception {ex}", agentId, connectionId, exception);

            //Make agent inactive
            //await _agentsService.SetAgentStatusAsync(Convert.ToInt32(agentId), (int)AgentStatusEnum.Inactive);
            await base.OnDisconnectedAsync(exception);
        }

        // Handle heartbeat (client response)
        public Task Heartbeat()
        {
            var connectionId = Context.ConnectionId;
            int agentId = 0;
            lock (connections)
                agentId = connections.FirstOrDefault(x => x.Value == connectionId).Key;

            _logger.LogInformation($"Heartbeat received in ConversationHub from agentId: {agentId} with connectionId {connectionId}");

            Clients.Client(connectionId).SendAsync(SignalREnum.HeartbeatAcknowledged.ToString(), $"Agent heartbeat sent with agent id - {agentId} and connection id - {connectionId}");

            return Task.CompletedTask;
        }
    }
}
