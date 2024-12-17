using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;

namespace WhatsAppAPISolutionDL.Hubs
{
    public class ConversationHub : Hub
    {
        public static readonly Dictionary<int, string> connections = new(); 
         
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
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove the Agent's connection ID when disconnected
            var connectionId = Context.ConnectionId; 
            lock (connections)
            {
                var agentId = connections.FirstOrDefault(x => x.Value == connectionId).Key;
                if (agentId != 0)
                {
                    connections.Remove(agentId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(int agentId, dynamic message)
        {
            // Send message to a specific user by their ID
            if (connections.TryGetValue(agentId, out var connectionId))
            {
                //Convert the message to string
                string messageStr = JsonConvert.SerializeObject(message);
                await Clients.Client(connectionId).SendAsync("NewMessagereceived", messageStr);
            }
        }
    }
}
