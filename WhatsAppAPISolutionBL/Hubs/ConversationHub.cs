using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionDL.Hubs
{
    public class ConversationHub : Hub
    {
        public static readonly Dictionary<int, string> connections = new();
        private readonly IAgentsService _agentsService;

        public ConversationHub(IAgentsService agentsService)
        {
            _agentsService = agentsService;
        }

        public override async Task OnConnectedAsync()
        {
            // Retrieve the Agent ID from the query string
            var agentId = Context.GetHttpContext()?.Request.Query["AgentId"];

            if (int.TryParse(agentId, out int parsedAgentId))
            {
                lock (connections)
                {
                    connections[parsedAgentId] = Context.ConnectionId;
                }

                //Make agent active
                await _agentsService.SetAgentStatusAsync(Convert.ToInt32(agentId), (int)AgentStatusEnum.Active);
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

            //Make agent inactive
            await _agentsService.SetAgentStatusAsync(Convert.ToInt32(agentId), (int)AgentStatusEnum.Inactive);
            await base.OnDisconnectedAsync(exception);
        } 
    }
}
