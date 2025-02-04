using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly IPermissionService _permissionService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<PermissionsController> _logger;
        private readonly IUserService _userService;

        public PermissionsController(IPermissionService permissionService,
            WhatsAppSolutionContext dbContext,
            ILogger<PermissionsController> logger,
            IUserService userService)
        {
            _permissionService = permissionService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getpermissionlist")]
        public async Task<ActionResult> GetPermissionListAsync(int RoleId = 0)
        {
            _logger.LogInformation("Calling api GetPermissionListAsync with clientId={clientId}, RoleId={RoleId}", clientId, RoleId);

            var res = await _permissionService.GetPermissionListAsync(clientId, RoleId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addpermission")]
        public async Task<IActionResult> AddPermissionAsync([FromBody] PermissionDto permission)
        {
            _logger.LogInformation("Calling api AddPermissionAsync with request={requst}", JsonConvert.SerializeObject(permission));

            if (permission == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            var response = await _permissionService.AddPermissionAsync(clientId, userId, permission);

            _logger.LogInformation("Received api AddPermissionAsync response with data={data}", JsonConvert.SerializeObject(response));

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
