using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.SystemActions;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SystemActionsController : ControllerBase
    {
        private readonly ISystemActionsService _systemActionsService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<SystemActionsController> _logger;

        public SystemActionsController(ISystemActionsService systemActionsService,
            WhatsAppSolutionContext dbContext,
            ILogger<SystemActionsController> logger)
        {
            _systemActionsService = systemActionsService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getsystemactionslist")]
        public async Task<ActionResult> GetSystemActionsListAsync(int ClientId, int SystemActionId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetSystemActionsListAsync with ClientId={ClientId}, SystemActionId={SystemActionId}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", ClientId, SystemActionId, SearchStr, SortBy, PageNo, PageSize);

            var res = await _systemActionsService.GetSystemActionsListAsync(ClientId, SystemActionId, PageNo, PageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsystemactionsbyid")]
        public async Task<ActionResult> GetSystemActionsByIdAsync(int clientId, int id)
        {
            _logger.LogInformation("Calling api GetSystemActionsByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (id <= 0)
            {
                return NotFound("not found");
            }

            var res = await _systemActionsService.GetSystemActionsByIdAsync(clientId, id);

            _logger.LogInformation("Received api GetSystemActionsByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addsystemactions")]
        public async Task<IActionResult> AddSystemActionsAsync([FromBody] SystemActionsDto systemActions)
        {
            _logger.LogInformation("Calling api AddSystemActionsAsync with request={requst}", JsonConvert.SerializeObject(systemActions));

            if (systemActions == null)
            {
                return BadRequest();
            }

            var response = await _systemActionsService.AddSystemActionsAsync(systemActions);

            _logger.LogInformation("Received api AddSystemActionsAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpPut("updatesystemactions")]
        public async Task<IActionResult> UpdateSystemActionsAsync(SystemActionsDto systemActions)
        {
            _logger.LogInformation("Calling api UpdateSystemActionsAsync with request={requst}", JsonConvert.SerializeObject(systemActions));

            if (systemActions == null)
            {
                return BadRequest();
            }

            var response = await _systemActionsService.UpdateSystemActionsAsync(systemActions);

            _logger.LogInformation("Received api UpdateSystemActionsAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpDelete("deletesystemactions")]
        public async Task<IActionResult> DeleteSystemActionsAsync(int systemActionsId)
        {
            _logger.LogInformation("Calling api DeleteSystemActionsAsync with SystemActionsId={SystemActionsId}", systemActionsId);

            if (systemActionsId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _systemActionsService.DeleteSystemActionsAsync(systemActionsId);

            _logger.LogInformation("Received api DeleteSystemActionsAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpGet("getsystemactions")]
        public async Task<IActionResult> GetSystemActionsAsync(int clientId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetSystemActionsAsync with clientId={ClientId}, searchStr={searchStr}", clientId, searchStr);

            var systemActions = await _systemActionsService.GetSystemActionsAsync(clientId, searchStr);

            if (systemActions == null || !systemActions.Any())
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
                Result = systemActions,
                Message = String.Empty
            });
        }
    }
}
