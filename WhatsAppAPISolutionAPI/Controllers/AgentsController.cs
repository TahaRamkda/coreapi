using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
        public async Task<ActionResult> GetAgentsListAsync(int ClientId, string SearchStr = "", int Status = 0)
        {
            var res = await _agentsService.GetAgentListAsync(ClientId, SearchStr, Status);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagentbyid")]
        public ActionResult GetAgentByIdAsync(int Id)
        {
            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Agents.Where(x => x.Id == Id).FirstOrDefault();

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
                return NotFound("not found");

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

    }
}
