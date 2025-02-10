using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IOneSignalService
    {
        Task SendConversationAssignedNotification(UAgentConversationList conversation);
        Task SendMessageReceivedNotification(ULatestConversationByConversation conversation);
        Task SendConversationUnAssignedNotification(int agentId);
    }
}
