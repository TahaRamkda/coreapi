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
            var response = await _dbContext2.Agents.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @SearchStr={SearchStr}, @Status={Status}").ToListAsync();
            return response;
        }
        public async Task<UResponse> AddAgentAsync(AgentDto agent)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={agent.ClientId}, @AgentFName={agent.AgentFName}, @AgentLName={agent.AgentLName}, @SenderIds={agent.SenderIds}, @ActionBy={agent.ActionBy}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> UpdateAgentAsync(AgentDto agent)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Update}, @Id={agent.Id}, @ClientId={agent.ClientId}, @AgentFName={agent.AgentFName}, @AgentLName={agent.AgentLName}, @SenderIds={agent.SenderIds}, @ActionBy={agent.ActionBy}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> DeleteAgentAsync(int AgentId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Delete}, @Id={AgentId}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> SetAgentStatusAsync(int id, int status)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.SetAgentStatus}, @Id={id}, @Status={status}").ToListAsync();
            return response[0];
        }
    }
}