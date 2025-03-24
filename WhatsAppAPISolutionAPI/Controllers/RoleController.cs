using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly IRoleService _rolesService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<RoleController> _logger;
        private readonly IUserService _userService;

        public RoleController(IRoleService rolesService,
            WhatsAppSolutionContext dbContext,
            ILogger<RoleController> logger,
            IUserService userService)
        {
            _rolesService = rolesService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getRolelist")]
        public async Task<ActionResult> GetRoleListAsync()
        {
            _logger.LogDebug("Calling api GetRoleListAsync with clientId={clientId}", clientId);

            var res = await _rolesService.GetRoleListAsync(clientId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getRolebyid")]
        public async Task<ActionResult> GetRoleByIdAsync(int id)
        {
            _logger.LogDebug("Calling api GetRoleByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (id <= 0)
                return NotFound("not found");

            var response = await _rolesService.GetRoleByIdAsync(clientId, id);

            _logger.LogDebug("Received api GetRoleByIdAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] RoleDto role)
        {
            _logger.LogDebug("Calling api AddRoleAsync with request={requst}", JsonConvert.SerializeObject(role));

            if (role == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (String.IsNullOrWhiteSpace(role.RoleName))
                return Ok(new ApiResult { Message = "Please enter role name" });

            var response = await _rolesService.AddRoleAsync(clientId, userId, role);

            _logger.LogDebug("Received api AddRoleAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updateRole")]
        public async Task<IActionResult> UpdateRoleAsync(RoleDto role)
        {
            _logger.LogDebug("Calling api UpdateRoleAsync with request={requst}", JsonConvert.SerializeObject(role));

            if (role == null && !ModelState.IsValid)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            var response = await _rolesService.UpdateRoleAsync(clientId, userId, role);

            _logger.LogDebug("Received api UpdateRoleAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRoleAsync(int RoleId)
        {
            _logger.LogDebug("Calling api DeleteRoleAsync with RoleId={RoleId}", RoleId);

            if (RoleId <= 0)
                return NotFound("not found");

            var response = await _rolesService.DeleteRoleAsync(RoleId);

            _logger.LogDebug("Received api DeleteRoleAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpGet("getroles")]
        public async Task<IActionResult> GetRolesAsync(string searchStr = "")
        {
            _logger.LogDebug("Calling api GetRolesAsync with clientId={clientId}, searchStr={searchStr}", clientId, searchStr);

            var models = await _rolesService.GetRolesAsync(clientId, searchStr);
            if (models == null || !models.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = models,
                Message = String.Empty
            });
        }
    }
}
