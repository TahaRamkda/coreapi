using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(IPermissionService permissionService,
            WhatsAppSolutionContext dbContext,
            ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getpermissionlist")]
        public async Task<ActionResult> GetPermissionListAsync(int client_Id, int role_Id = 0)
        {
            var res = await _permissionService.GetPermissionListAsync(client_Id, role_Id);
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
            if (permission == null)
            {
                return BadRequest();
            }

            var response = await _permissionService.AddPermissionAsync(permission);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
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
    }
}
