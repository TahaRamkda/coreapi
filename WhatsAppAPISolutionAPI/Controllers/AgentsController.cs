using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

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
            if (Id <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });

            var response = await _agentsService.DeleteAgentAsync(Id);
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
        public async Task<IActionResult> SetAgentStatusAsync(int id, int status)
        {
            if (id <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select agent"
                });

            var response = await _agentsService.SetAgentStatusAsync(id, status);
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
            var res = await _agentsService.GetAgentTimingListAsync(clientId, agentId);
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
            var agents = await _agentsService.GetAgentsAsync(clientId, senderId, searchStr);
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
            var res = await _agentsService.GetAgentSupervisorReportListAsync(clientId, searchStr, status, senderId, fromDate, toDate, sortBy, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
