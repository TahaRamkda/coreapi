using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class AgentsService : IAgentsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public AgentsService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UAgent>> GetAgentListAsync(int ClientId, string SearchStr = "", int Status = 0)
        {
            var query = string.Format(@"exec usp_Agents_Ops @ActionId={0}, @ClientId={1}, @SearchStr='{2}', @Status={3}", (int)CrudEnum.List, ClientId, SearchStr, Status);
            var response = await _dbContext2.Agents.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddAgentAsync(AgentDto agent)
        {
            var query = string.Format(@"exec usp_Agents_Ops @ActionId={0}, @ClientId={1}, @AgentFName='{2}', @AgentLName='{3}', @SenderIds='{4}', @ActionBy={5}", (int)CrudEnum.Add, agent.ClientId, agent.AgentFName, agent.AgentLName, agent.SenderIds, agent.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateAgentAsync(AgentDto agent)
        {
            var query = string.Format(@"exec usp_Agents_Ops @ActionId={0}, @Id={1}, @ClientId={2}, @AgentFName='{3}', @AgentLName='{4}', @SenderIds='{5}', @ActionBy={6}", (int)CrudEnum.Update, agent.Id, agent.ClientId, agent.AgentFName, agent.AgentLName, agent.SenderIds, agent.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteAgentAsync(int AgentId)
        {
            var query = string.Format(@"exec usp_Agents_Ops @ActionId={0}, @Id={1}", (int)CrudEnum.Delete, AgentId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> SetAgentStatusAsync(int id, int status)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.SetAgentStatus}, @Id={id}, @Status={status}").ToListAsync();

            return response[0];
        }
    }
}