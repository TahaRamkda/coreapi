using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IAgentsService
    {
        public Task<List<UAgent>> GetAgentListAsync(int ClientId, string SearchStr = "", int Status = 0);
        public Task<UResponse> AddAgentAsync(AgentDto agent);
        public Task<UResponse> UpdateAgentAsync(AgentDto agent);
        public Task<UResponse> DeleteAgentAsync(int AgentId);
        public Task<UResponse> SetAgentStatusAsync(int Id, int Status);
    }
}
