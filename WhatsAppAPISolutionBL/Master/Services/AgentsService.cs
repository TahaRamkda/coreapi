using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class AgentsService : IAgentsService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IImportManager _importManager;
        private readonly ILogger<AgentsService> _logger;

        public AgentsService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IImportManager importManager,
            ILogger<AgentsService> logger)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _importManager = importManager;
            _logger = logger;
        }

        public async Task<List<UAgent>> GetAgentListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.Agents.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @SearchStr={searchStr ?? ""}, @Status={status}, @SenderId={senderId}, @SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}").ToListAsync();
            return response;
        }
        public async Task<UResponse> AddAgentAsync(int clientId, int userId, AgentDto agent)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId},@UserName={agent.UserName}, @Password={agent.Password},@AgentFName={agent.AgentFName}, @AgentLName={agent.AgentLName}, @PreferredLanguage={agent.PreferredLanguage},@SenderIds={agent.SenderIds}, @ActionBy={userId}, @AgentFNameAR={agent.AgentFNameAR}, @AgentLNameAR={agent.AgentLNameAR}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> UpdateAgentAsync(int clientId, int userId, AgentDto agent)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Update}, @Id={agent.Id}, @ClientId={clientId}, @AgentFName={agent.AgentFName}, @AgentLName={agent.AgentLName}, @PreferredLanguage={agent.PreferredLanguage},@SenderIds={agent.SenderIds}, @ActionBy={userId}, @AgentFNameAR={agent.AgentFNameAR}, @AgentLNameAR={agent.AgentLNameAR}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> DeleteAgentAsync(int AgentId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.Delete}, @Id={AgentId}").ToListAsync();
            return response[0];
        }
        public async Task<UResponse> SetAgentStatusAsync(int agentId, int status)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.SetAgentStatus}, @Id={agentId}, @Status={status}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> SetAgentDisableAsync(int clientId, int agentId, bool disable)
        {
            int status = disable ? 1 : 0;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.SetAgentEnableDisable}, @ClientId={clientId}, @Id={agentId}, @IsDisabled={status}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> AddAgentTimingsAsync(int clientId, int userId, AgentTimingDto model)
        {
            var timings = JsonConvert.SerializeObject(model.Timings);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec Usp_AgentTimings_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @AgentId={model.AgentId}, @JsonData={timings}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }
        public async Task<List<UAgentTiming>> GetAgentTimingListAsync(int clientId, int agentId)
        {
            var response = await _dbContext2.AgentTimings.FromSqlInterpolated($"exec Usp_AgentTimings_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @AgentId={agentId}").ToListAsync();
            return response;
        }

        public async Task<List<UEntityDto>> GetAgentsAsync(int clientId, int senderId = 0, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId}, @SenderId={senderId}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UEntityDto>> GetActiveAgentsAsync(int clientId, int senderId = 0, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.GetActiveAgents}, @ClientId={clientId}, @SenderId={senderId}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<UAgentDetail> GetAgentByIdAsync(int clientId, int agentId)
        {
            var response = await _dbContext2.AgentDetails.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}, @Id={agentId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }

        public async Task<List<UAgentSupervisorReport>> GetAgentSupervisorReportListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.AgentSupervisorReports.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.GetAgentSupervisorReport}, @ClientId={clientId}, @SearchStr={searchStr ?? ""}, @Status={status}, @SenderId={senderId}, @FromDate={fromDate}, @ToDate={toDate}, @SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<List<UAgentSupervisorReport>> GetAgentDetailSupervisorReportListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.AgentSupervisorReports.FromSqlInterpolated($"exec usp_Agents_Ops @ActionId={(int)CrudEnum.GetAgentDetailSupervisorReport}, @ClientId={clientId}, @SearchStr={searchStr ?? ""}, @Status={status}, @SenderId={senderId}, @FromDate={fromDate}, @ToDate={toDate}, @SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<UAgentStat> GetAgentStatsAsync(int clientId, int agentId, int senderId = 0)
        {
            var response = await _dbContext2.AgentStats.FromSqlInterpolated($"exec usp_Conversations_AgentStats @ClientId={clientId}, @AgentId={agentId}, @SenderId={senderId}").ToListAsync();

            if (response != null && response.Count > 0)
                return response[0];

            return null;
        }

        public async Task<UResponse> ImportBulkAgentTimings(int clientId, int userId, ImportBulkAgentTimingDto model)
        {
            var bulkTimings = _importManager.ImportBulkAgentTimingsFromXlsx(model.File.OpenReadStream());
            if (bulkTimings == null || !bulkTimings.Any())
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot import bulk timings, No timings found"
                };
            }

            var bulkTimingsJson = System.Text.Json.JsonSerializer.Serialize(bulkTimings);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_AgentTimings_BulkUpload @BulkAgentTimings={bulkTimingsJson}, @ClientId={clientId}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<bool> IsAgentOneSignalEnabled(int? clientId, int? senderId = 0)
        {
            _logger.LogInformation("Calling api IsAgentOneSignalEnabled with clientId={clientId}, senderId={senderId}", clientId, senderId);
            string keyNames = CommonEnum.IsOneSignalEnabled.ToString();
            var response = await _dbContext2.AppSetting.FromSqlInterpolated($"exec usp_Appsettings_Ops @ActionId={(int)CrudEnum.GetAppSettings}, @KeyName={keyNames}, @ClientId={clientId}, @SenderId={senderId}").ToListAsync();
            _logger.LogInformation("Recieved api IsAgentOneSignalEnabled response with response={response}", JsonConvert.SerializeObject(response));
            if (!response.Any()) return false;
            else return response[0].Val == "0" ? false : true;
        }
    }
}