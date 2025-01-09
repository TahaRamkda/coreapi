using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Events;
using System.Drawing.Printing;
using System.Globalization;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentsService _agentsService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(IAgentsService agentsService,
            WhatsAppSolutionContext dbContext,
            ILogger<AgentsController> logger)
        {
            _agentsService = agentsService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getagentlist")]
        public async Task<ActionResult> GetAgentsListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
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
        public async Task<ActionResult> GetAgentByIdAsync(int clientId, int agentId)
        {
            _logger.LogInformation("Calling api GetAgentByIdAsync with clientId={clientId}, agentId={agentId}", clientId, agentId);

            if (clientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter client id"
                });
            }

            if (agentId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter agent id"
                });
            }

            var response = await _agentsService.GetAgentByIdAsync(clientId, agentId);

            _logger.LogInformation("Received api GetAgentByIdAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Result = "",
                    Message = "No record found with this id"
                });
            }

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
            {
                return BadRequest();
            }

            if (agent.ClientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter client id"
                });
            }

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter agent first name"
                });
            }

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter agent last name"
                });
            }

            if (String.IsNullOrWhiteSpace(agent.UserName))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter user name"
                });
            }

            agent.UserName = agent.UserName.Trim();
            agent.Password = agent.Password.Trim();
            if (!CommonHelper.IsValidUsername(agent.UserName))
            {
                return Ok(new ApiResult
                {
                    Message = "The username should contains only alphanumeric value and no special character other than - and ."
                });
            }

            if (String.IsNullOrWhiteSpace(agent.Password))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter password"
                });
            }

            var response = await _agentsService.AddAgentAsync(agent);

            _logger.LogInformation("Received api AddAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
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
            {
                return BadRequest();
            }

            if (agent.ClientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter client id"
                });
            }

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter agent first name"
                });
            }

            if (String.IsNullOrWhiteSpace(agent.AgentFName))
            {
                return Ok(new ApiResult
                {
                    Message = "Please enter agent last name"
                });
            }

            var response = await _agentsService.UpdateAgentAsync(agent);

            _logger.LogInformation("Received api UpdateAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
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
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });

            var response = await _agentsService.DeleteAgentAsync(Id);

            _logger.LogInformation("Received api DeleteAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpPost("setagentstatus")]
        public async Task<IActionResult> SetAgentStatusAsync(int agentId, int status)
        {
            _logger.LogInformation("Calling api SetAgentStatusAsync with agentId={agentId}, status={status}", agentId, status);

            if (agentId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });

            var response = await _agentsService.SetAgentStatusAsync(agentId, status);

            _logger.LogInformation("Received api SetAgentStatusAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Status Updated successfully"
            });
        }

        [HttpPost("setagentdisable")]
        public async Task<IActionResult> SetAgentDisableAsync(int clientId, int agentId, bool disable)
        {
            _logger.LogInformation("Calling api SetAgentDisableAsync with clientId={clientId}, agentId={agentId}, disable={disable}", clientId, agentId, disable);

            if (clientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select client"
                });

            if (agentId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });

            var response = await _agentsService.SetAgentDisableAsync(clientId, agentId, disable);

            _logger.LogInformation("Received api SetAgentDisableAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

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

            if (model.ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select client"
                });

            if (model.AgentId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });


            if (model.Timings == null || model.Timings.Count == 0)
                return Ok(new ApiResult
                {
                    Message = "Please enter timings"
                });

            var response = await _agentsService.AddAgentTimingsAsync(model);

            _logger.LogInformation("Received api AddAgentTimingsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpGet("getagenttiminglist")]
        public async Task<ActionResult> GetAgentTimingListAsync(int clientId, int agentId = 0)
        {
            _logger.LogInformation("Calling api GetAgentTimingListAsync with clientId={clientId}, agentId={agentId}", clientId, agentId);

            var res = await _agentsService.GetAgentTimingListAsync(clientId, agentId);

            _logger.LogInformation("Received api GetAgentTimingListAsync response with data={data}", JsonConvert.SerializeObject(res));

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagents")]
        public async Task<IActionResult> GetAgentsAsync(int clientId, int senderId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetAgentsAsync with clientId={clientId}, senderId={senderId}, searchStr={searchStr}", clientId, senderId, searchStr);

            var agents = await _agentsService.GetAgentsAsync(clientId, senderId, searchStr);

            _logger.LogInformation("Received api GetAgentsAsync response with data={data}", JsonConvert.SerializeObject(agents));

            if (agents == null || !agents.Any())
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = null,
                    Message = "No records found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = agents,
                Message = String.Empty
            });
        }

        [HttpGet("getagentsupervisorreport")]
        public async Task<ActionResult> GetAgentSupervisorReportListAsync(int clientId, string searchStr = "", int status = 0, int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentSupervisorReportListAsync with clientId={clientId}, searchStr={searchStr}, status={status}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            var res = await _agentsService.GetAgentSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);

            _logger.LogInformation("Received api GetAgentSupervisorReportListAsync response with data={data}", JsonConvert.SerializeObject(res));
            
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("GetAgentStats")]
        public async Task<ActionResult> GetAgentStatsAsync(int clientId, int agentId, int senderId = 0)
        {
            _logger.LogInformation("Calling api GetAgentStatsAsync with clientId={clientId}, agentId={agentId}, senderId={senderId}", clientId, agentId, senderId);

            var res = await _agentsService.GetAgentStatsAsync(clientId, agentId, senderId);

            _logger.LogInformation("Received api GetAgentStatsAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult { Message = "Cannot fetch agent stats" });
            }

            return Ok(new ApiResult
            {
                Success = true,
                StatusCode = 200,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

    }
}
