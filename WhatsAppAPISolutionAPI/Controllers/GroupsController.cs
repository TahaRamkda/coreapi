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
        public async Task<ActionResult> GetGroupsListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var res = await _groupService.GetGroupListAsync(ClientId, SearchStr, SortBy, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getgroupbyid")]
        public ActionResult GetGroupByIdAsync(int Id)
        {
            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Groups.Where(x => x.GroupId == Id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Success = false,
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
            return Ok(new ApiResult
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
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletegroup")]
        public async Task<IActionResult> DeleteGroupAsync(int GroupId)
        {
            if (GroupId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _groupService.DeleteGroupAsync(GroupId);
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
                Message = "Data deleted successfully"
            });
        }
    }
}
