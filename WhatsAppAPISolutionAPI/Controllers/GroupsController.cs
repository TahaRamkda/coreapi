using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService,
            WhatsAppSolutionContext dbContext,
            ILogger<GroupsController> logger)
        {
            _groupService = groupService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("getgroupslist")]
        public async Task<ActionResult> GetGroupsListAsync(int client_Id)
        {
            var res = await _groupService.GetGroupListAsync(client_Id);
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getgroupbyid")]
        public ActionResult GetGroupByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Groups.Where(x => x.GroupId == id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addGroup")]
        public async Task<IActionResult> AddGroupAsync([FromBody] GroupDto group)
        {
            if (group == null)
            {
                return BadRequest();
            }

            var response = await _groupService.AddGroupAsync(group);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updategroup")]
        public async Task<IActionResult> UpdateGroupAsync(GroupDto group)
        {
            if (group == null)
            {
                return BadRequest();
            }

            var response = await _groupService.UpdateGroupAsync(group);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletegroup")]
        public async Task<IActionResult> DeleteGroupAsync(int group_Id)
        {
            if (group_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _groupService.DeleteGroupAsync(group_Id);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }
    }
}
