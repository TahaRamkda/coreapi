using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AgentsController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly IAgentsService _agentsService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<AgentsController> _logger;
        private readonly IUserService _userService;
        private readonly IExportManager _exportManager;

        //test github
        public AgentsController(IAgentsService agentsService,
            WhatsAppSolutionContext dbContext,
            ILogger<AgentsController> logger,
            IUserService userService,
            IExportManager exportManager)
        {
            _agentsService = agentsService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
            _exportManager = exportManager;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getagentlist")]
        public async Task<ActionResult> GetAgentsListAsync(string searchStr = "", int status = 0, int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentsListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, searchStr, status, senderId, sortBy, pageNo, pageSize);

            var res = await _agentsService.GetAgentListAsync(clientId, searchStr, status, senderId, sortBy, pageNo, pageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagentbyid")]
        public async Task<ActionResult> GetAgentByIdAsync(int agentId)
        {
            _logger.LogInformation("Calling api GetAgentByIdAsync with clientId={clientId}, agentId={agentId}", clientId, agentId);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (agentId <= 0)
                return Ok(new ApiResult { Message = "Please enter agent id" });

            var response = await _agentsService.GetAgentByIdAsync(clientId, agentId);

            _logger.LogInformation("Received api GetAgentByIdAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addagent")]
        public async Task<IActionResult> AddAgentAsync([FromBody] AgentDto agent)
        {
            _logger.LogInformation("Calling api AddAgentAsync with request={requst}", JsonConvert.SerializeObject(agent));

            if (agent == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
                return Ok(new ApiResult { Message = "Please enter agent first name" });

            if (String.IsNullOrWhiteSpace(agent.AgentLName))
                return Ok(new ApiResult { Message = "Please enter agent last name" });

            if (String.IsNullOrWhiteSpace(agent.UserName))
                return Ok(new ApiResult { Message = "Please enter user name" });

            agent.UserName = agent.UserName.Trim();
            agent.Password = agent.Password.Trim();

            if (!CommonHelper.IsValidUsername(agent.UserName))
                return Ok(new ApiResult { Message = "The username should contains only alphanumeric value and no special character other than - and ." });

            if (String.IsNullOrWhiteSpace(agent.Password))
                return Ok(new ApiResult { Message = "Please enter password" });

            var response = await _agentsService.AddAgentAsync(clientId, userId, agent);

            _logger.LogInformation("Received api AddAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updateagent")]
        public async Task<IActionResult> UpdateAgentAsync(AgentDto agent)
        {
            _logger.LogInformation("Calling api UpdateAgentAsync with request={requst}", JsonConvert.SerializeObject(agent));

            if (agent == null)
                return BadRequest();

            //agent.ClientId = clientId;
            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
                return Ok(new ApiResult { Message = "Please enter agent first name" });

            if (String.IsNullOrWhiteSpace(agent.AgentLName))
                return Ok(new ApiResult { Message = "Please enter agent last name" });

            var response = await _agentsService.UpdateAgentAsync(clientId, userId, agent);

            _logger.LogInformation("Received api UpdateAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deleteagent")]
        public async Task<IActionResult> DeleteAgentAsync(int Id)
        {
            _logger.LogInformation("Calling api DeleteAgentAsync with id={Id}", Id);

            if (Id <= 0)
                return Ok(new ApiResult { Message = "Please select agent" });

            var response = await _agentsService.DeleteAgentAsync(Id);

            _logger.LogInformation("Received api DeleteAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpGet("setagentstatus")]
        public async Task<IActionResult> SetAgentStatusAsync(int agentId, int status)
        {
            _logger.LogInformation("Calling api SetAgentStatusAsync with agentId={agentId}, status={status}", agentId, status);

            if (agentId <= 0)
                return Ok(new ApiResult { Message = "Please select agent" });

            var response = await _agentsService.SetAgentStatusAsync(agentId, status);

            _logger.LogInformation("Received api SetAgentStatusAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Status Updated successfully"
            });
        }

        [HttpGet("setagentdisable")]
        public async Task<IActionResult> SetAgentDisableAsync(int agentId, bool disable)
        {
            _logger.LogInformation("Calling api SetAgentDisableAsync with clientId={clientId}, agentId={agentId}, disable={disable}", clientId, agentId, disable);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (agentId <= 0)
                return Ok(new ApiResult { Message = "Please select agent" });

            var response = await _agentsService.SetAgentDisableAsync(clientId, agentId, disable);

            _logger.LogInformation("Received api SetAgentDisableAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Status Updated successfully"
            });
        }

        [HttpPost("addagenttimings")]
        public async Task<IActionResult> AddAgentTimingsAsync(AgentTimingDto model)
        {
            _logger.LogInformation("Calling api AddAgentTimingsAsync with request={request}", JsonConvert.SerializeObject(model));
            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (model.AgentId <= 0)
                return Ok(new ApiResult { Message = "Please select agent" });

            if (model.Timings == null || model.Timings.Count == 0)
                return Ok(new ApiResult { Message = "Please enter timings" });

            var response = await _agentsService.AddAgentTimingsAsync(clientId, userId, model);

            _logger.LogInformation("Received api AddAgentTimingsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpGet("getagenttiminglist")]
        public async Task<ActionResult> GetAgentTimingListAsync(int agentId = 0)
        {
            _logger.LogInformation("Calling api GetAgentTimingListAsync with clientId={clientId}, agentId={agentId}", clientId, agentId);

            var res = await _agentsService.GetAgentTimingListAsync(clientId, agentId);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagents")]
        public async Task<IActionResult> GetAgentsAsync(int senderId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetAgentsAsync with clientId={clientId}, senderId={senderId}, searchStr={searchStr}", clientId, senderId, searchStr);

            var agents = await _agentsService.GetAgentsAsync(clientId, senderId, searchStr);

            if (agents == null || !agents.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = agents,
                Message = String.Empty
            });
        }

        [HttpGet("getactiveagents")]
        public async Task<IActionResult> GetActiveAgentsAsync(int senderId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetActiveAgentsAsync with clientId={clientId}, senderId={senderId}, searchStr={searchStr}", clientId, senderId, searchStr);

            var agents = await _agentsService.GetActiveAgentsAsync(clientId, senderId, searchStr);

            if (agents == null || !agents.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = agents,
                Message = String.Empty
            });
        }

        [HttpGet("getagentsupervisorreport")]
        public async Task<ActionResult> GetAgentSupervisorReportListAsync(string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentSupervisorReportListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            var res = await _agentsService.GetAgentSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("exportagentsupervisorreport")]
        public async Task<ActionResult> ExportAgentSupervisorReportListAsync(string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0)
        {
            _logger.LogInformation("Calling api ExportAgentSupervisorReportListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}", clientId, searchStr, status, senderId, fromDate, toDate, sortBy);

            var res = await _agentsService.GetAgentSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy);
            var bytes = _exportManager.ExportAgentSupervisorReportToXlsx(res);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AgentSupervisorReport.xlsx");
        }

        [HttpGet("getagentdetailsupervisorreport")]
        public async Task<ActionResult> GetAgentDetailSupervisorDetailReportListAsync(string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentDetailSupervisorReportListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            var res = await _agentsService.GetAgentDetailSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getexportagentdetailsupervisorreport")]
        public async Task<ActionResult> ExportAgentDetailSupervisorDetailReportListAsync(string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0)
        {
            _logger.LogInformation("Calling api ExportAgentDetailSupervisorReportListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}", clientId, searchStr, status, senderId, fromDate, toDate, sortBy);

            var res = await _agentsService.GetAgentDetailSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy);
            var bytes = _exportManager.ExportAgentDetailSupervisorReportToXlsx(res);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AgentDetailSupervisorReport.xlsx");
        }

        [HttpGet("getagentstats")]
        public async Task<ActionResult> GetAgentStatsAsync(int agentId, int senderId = 0)
        {
            _logger.LogInformation("Calling api GetAgentStatsAsync with clientId={clientId}, agentId={agentId}, senderId={senderId}", clientId, agentId, senderId);

            var res = await _agentsService.GetAgentStatsAsync(clientId, agentId, senderId);

            if (res == null)
                return Ok(new ApiResult { Message = "Cannot fetch agent stats" });

            return Ok(new ApiResult
            {
                Success = true,
                StatusCode = 200,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("importbulkagenttimings")]
        public async Task<IActionResult> ImportBulkAgentTimingsAsync([FromForm] ImportBulkAgentTimingDto model)
        {
            _logger.LogInformation("Calling api ImportBulkAgentTimingsAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (model.File == null || model.File.Length <= 0)
                return Ok(new ApiResult { Message = "No file found" });

            var response = await _agentsService.ImportBulkAgentTimings(clientId, userId, model);

            _logger.LogInformation("Received api ImportBulkAgentTimingsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }
    }
}
