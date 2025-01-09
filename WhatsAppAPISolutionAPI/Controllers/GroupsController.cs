using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Globalization;
using System.Text.RegularExpressions;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Group;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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
            _logger.LogInformation("Calling api GetGroupsListAsync with clientId={clientId}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", ClientId, SearchStr, SortBy, PageNo, PageSize);

            var res = await _groupService.GetGroupListAsync(ClientId, SearchStr, SortBy, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getgroupbyid")]
        public async Task<ActionResult> GetGroupByIdAsync(int clientId, int id)
        {
            _logger.LogInformation("Calling api GetGroupByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (id <= 0)
                return Ok(new ApiResult { Message = "not found" });

            var response = await _groupService.GetGroupByIdAsync(clientId, id);

            _logger.LogInformation("Received api GetGroupByIdAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpPost("addGroup")]
        public async Task<IActionResult> AddGroupAsync([FromBody] GroupDto group)
        {
            _logger.LogInformation("Calling api AddGroupAsync with request={requst}", JsonConvert.SerializeObject(group));

            if (group == null)
            {
                return BadRequest();
            }

            var response = await _groupService.AddGroupAsync(group);

            _logger.LogInformation("Received api AddGroupAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpPut("updategroup")]
        public async Task<IActionResult> UpdateGroupAsync(GroupDto group)
        {
            _logger.LogInformation("Calling api UpdateGroupAsync with request={requst}", JsonConvert.SerializeObject(group));

            if (group == null)
            {
                return BadRequest();
            }

            var response = await _groupService.UpdateGroupAsync(group);

            _logger.LogInformation("Received api UpdateGroupAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpDelete("deletegroup")]
        public async Task<IActionResult> DeleteGroupAsync(int GroupId)
        {
            _logger.LogInformation("Calling api DeleteGroupAsync with GroupId={GroupId}", GroupId);

            if (GroupId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _groupService.DeleteGroupAsync(GroupId);

            _logger.LogInformation("Received api DeleteGroupAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpGet("getgroups")]
        public async Task<IActionResult> GetGroupsAsync(int clientId, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetGroupsAsync with clientId={clientId}, searchStr={searchStr}", clientId, searchStr);

            var groups = await _groupService.GetGroupsAsync(clientId, searchStr);
            if (groups == null || !groups.Any())
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
                Result = groups,
                Message = String.Empty
            });
        }
    }
}
