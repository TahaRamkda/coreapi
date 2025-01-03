using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _rolesService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService rolesService,
            WhatsAppSolutionContext dbContext,
            ILogger<RoleController> logger)
        {
            _rolesService = rolesService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getRolelist")]
        public async Task<ActionResult> GetRoleListAsync(int ClientId)
        {
            var res = await _rolesService.GetRoleListAsync(ClientId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getRolebyid")]
        public ActionResult GetRoleByIdAsync(int clientId, int id)
        {
            if (id <= 0)
                return NotFound("not found");

            var response = _rolesService.GetRoleByIdAsync(clientId, id);
            if (response == null)
            {
                return Ok(new ApiResult
                {
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

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] RoleDto role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            var response = await _rolesService.AddRoleAsync(role);
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

        [HttpPut("updateRole")]
        public async Task<IActionResult> UpdateRoleAsync(RoleDto role)
        {
            if (role == null && !ModelState.IsValid)
            {
                return BadRequest();
            }

            var response = await _rolesService.UpdateRoleAsync(role);
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

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRoleAsync(int RoleId)
        {
            if (RoleId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _rolesService.DeleteRoleAsync(RoleId);
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

        [HttpGet("getroles")]
        public async Task<IActionResult> GetRolesAsync(int clientId, string searchStr = "")
        {
            var models = await _rolesService.GetRolesAsync(clientId, searchStr);
            if (models == null || !models.Any())
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
                Result = models,
                Message = String.Empty
            });
        }
    }
}
