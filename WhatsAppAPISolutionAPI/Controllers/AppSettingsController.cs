using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionDL.Dto.AppSettings;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Group;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsService _appSettingsService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly int clientId;
        private readonly ILogger<AppSettingsController> _logger; 
        private readonly IUserService _userService;

        public AppSettingsController(WhatsAppSolutionContext dbContext, ILogger<AppSettingsController> logger, IAppSettingsService appSettingsService, IUserService _userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettingsService = appSettingsService;
            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getappsettinglist")]
        public async Task<ActionResult> GetAppSettingListAsync(string SearchStr = "",  int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogDebug("Calling api GetAppSettingsAsync with clientId={clientId}, searchStr={searchStr}, pageNo={pageNo}, pageSize={pageSize}", clientId, SearchStr,  PageNo, PageSize);

            var res = await _appSettingsService.GetAppSettingsAsync(clientId, SearchStr, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getappsettingbyid")]
        public async Task<ActionResult> GetAppSettingById(int id)
        {
            _logger.LogDebug("Calling api GetAppSettingsByIdAsync with clientId={clientId}, id={id}", id, clientId);

            if (id <= 0)
                return Ok(new ApiResult { Message = "not found" });

            var response = await _appSettingsService.GetAppSettingsByIdAsync(id, clientId);

            _logger.LogDebug("Received api GetAppSettingsByIdAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addAppsettings")]
        public async Task<ActionResult> AddAppSettings(int userId, [FromBody] AppSettingsDto Appsetting)
        {
            _logger.LogInformation("Calling api AddAppSettingsAsync with request={request}", JsonConvert.SerializeObject(Appsetting));

            if (Appsetting == null)
                return BadRequest(new { Message = "Please enter valid data" });

            if (string.IsNullOrWhiteSpace(Appsetting.KeyName))
                return Ok(new { Message = "Please enter key name" });

            var response = await _appSettingsService.AddAppSettingsAsync(clientId, userId, Appsetting);

            _logger.LogDebug("Received api AddAppSettingsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updateappsettings")]
        public async Task<IActionResult> UpdateAppSettings(int userId, [FromBody] AppSettingsDto appSettings)
        {
            _logger.LogDebug("Calling api UpdateAppSettingsAsync with request={request}", JsonConvert.SerializeObject(appSettings));

            if (appSettings == null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(appSettings.KeyName))
                return Ok(new ApiResult { Message = "Please enter group name" });

            var response = await _appSettingsService.UpdateAppSettingsAsync(clientId, userId, appSettings);

            _logger.LogDebug("Received api UpdateAppSettingsAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deleteappsetting")]
        public async Task<IActionResult> DeleteAppSettingAsync(int Id)
        {
            _logger.LogDebug("Calling api DeleteAppSettingAsync with Id={Id}", Id);

            if (Id <= 0)
                return NotFound("not found");

            var response = await _appSettingsService.DeleteAppSettingsAsync(Id);

            _logger.LogDebug("Received api DeleteAppSettingAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpGet("getappsettings")]
        public async Task<IActionResult> GetAppSettingAsync(string searchStr = "")
        {
            _logger.LogDebug("Calling api GetAppSettingAsync with clientId={clientId}, searchStr={searchStr}", clientId, searchStr);

            var groups = await _appSettingsService.GetAllAppSettingsAsync(clientId, searchStr);
            if (groups == null || !groups.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = groups,
                Message = string.Empty
            });
        }
    }
}
