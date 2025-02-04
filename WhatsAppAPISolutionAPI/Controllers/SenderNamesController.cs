using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Agent;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SenderNamesController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly ISenderNameService _senderNameService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<SenderNamesController> _logger;
        private readonly IMediaService _mediaService;
        private readonly IUserService _userService;

        public SenderNamesController(ISenderNameService senderNameService,
            WhatsAppSolutionContext dbContext,
            ILogger<SenderNamesController> logger,
            IMediaService mediaService,
            IUserService userService)
        {
            _senderNameService = senderNameService;
            _dbContext = dbContext;
            _logger = logger;
            _mediaService = mediaService;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getsenderNameslist")]
        public async Task<ActionResult> GetSenderNamesListAsync()
        {
            _logger.LogInformation("Calling api GetSenderNamesListAsync with clientId={clientId}", clientId);

            var res = await _senderNameService.GetSenderNameListAsync(clientId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsenderNamebyid")]
        public async Task<ActionResult> GetSenderNameByIdAsync(int id)
        {
            _logger.LogInformation("Calling api GetSenderNameByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (id <= 0)
            {
                return Ok(new ApiResult { Message = "not found" });
            }

            var res = await _senderNameService.GetSenderNameByIdAsync(clientId, id);

            _logger.LogInformation("Received api GetSenderNameByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addsenderName")]
        public async Task<ActionResult> AddSenderNameAsync([FromForm] SenderNameDto model)
        {
            _logger.LogInformation("Calling api AddSenderNameAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                if (!_mediaService.CheckAllowedImageType(extension))
                {
                    return Ok(new ApiResult
                    {
                        Message = $"Cannot upload media with file extension {extension}"
                    });
                }

                var mediaUpload = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ActionBy = userId,
                    ClientId = clientId,
                    UploadToFacebook = false,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Admin
                });

                if (mediaUpload.Status <= 0)
                    return Ok(new ApiResult { Message = mediaUpload.Message });

                model.MediaId = mediaUpload.Id;
            }

            var response = await _senderNameService.AddSenderNameAsync(clientId, userId, model);

            _logger.LogInformation("Received api AddSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updatesenderName")]
        public async Task<IActionResult> UpdateSenderNameAsync([FromForm] SenderNameDto model)
        {
            _logger.LogInformation("Calling api UpdateSenderNameAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                if (!_mediaService.CheckAllowedImageType(extension))
                    return Ok(new ApiResult { Message = $"Cannot upload media with file extension {extension}" });

                var mediaUpload = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ActionBy = userId,
                    ClientId = clientId,
                    UploadToFacebook = false,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Admin
                });

                if (mediaUpload.Status <= 0)
                    return Ok(new ApiResult { Message = mediaUpload.Message });

                model.MediaId = mediaUpload.Id;
            }

            var response = await _senderNameService.UpdateSenderNameAsync(clientId, userId, model);

            _logger.LogInformation("Received api UpdateSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletesenderName")]
        public async Task<IActionResult> DeleteSenderNameAsync(int SenderNameId)
        {
            _logger.LogInformation("Calling api DeleteSenderNameAsync with SenderNameId={SenderNameId}", SenderNameId);

            if (SenderNameId <= 0)
                return NotFound("not found");

            var response = await _senderNameService.DeleteSenderNameAsync(SenderNameId);

            _logger.LogInformation("Received api DeleteSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [AllowAnonymous]
        [HttpGet("getsendernameinformation")]
        public ActionResult GetSenderNameInformationAsync(int ClientId, int SenderNameId)
        {
            _logger.LogInformation("Calling api GetSenderNameInformationAsync with ClientId={ClientId}, SenderNameId={SenderNameId}", ClientId, SenderNameId);

            if (ClientId <= 0)
            {
                return NotFound("not found");
            }

            var response = (from a in _dbContext.SenderNames
                            join b in _dbContext.Clients on a.ClientId equals b.ClientId
                            where a.ClientId == ClientId && a.SenderId == SenderNameId && a.RecordStatus != -1 && b.RecordStatus != -1
                            select new
                            {
                                ClientId = a.ClientId,
                                SenderId = a.SenderId,
                                SenderName = a.SenderName1,
                                PhoneNumberId = a.PhoneNumberId,
                                PhoneNumber = a.PhoneNumber,
                                BusinessAccountId = a.BusinessAccountId,
                                AccessToken = b.AccessToken,
                                AppId = b.AppId,
                                BusinessId = b.BusinessId
                            }).FirstOrDefault();

            _logger.LogInformation("Received api GetSenderNameInformationAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null)
                return Ok(new ApiResult { Message = "No record found with this id" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsendernames")]
        public async Task<IActionResult> GetSenderNamesAsync(string searchStr = "")
        {
            _logger.LogInformation("Calling api GetSenderNamesAsync with ClientId={ClientId}, searchStr={searchStr}", clientId, searchStr);

            var models = await _senderNameService.GetSenderNamesAsync(clientId, searchStr);
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
