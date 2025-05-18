using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Client;

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
            _logger.LogDebug("Calling api GetSenderNamesListAsync with clientId={clientId}", clientId);

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
            _logger.LogDebug("Calling api GetSenderNameByIdAsync with clientId={clientId}, id={id}", clientId, id);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (id <= 0)
            {
                return Ok(new ApiResult { Message = "not found" });
            }

            var res = await _senderNameService.GetSenderNameByIdAsync(clientId, id);

            _logger.LogDebug("Received api GetSenderNameByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

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
            _logger.LogDebug("Calling api AddSenderNameAsync with request={requst}", JsonConvert.SerializeObject(model));

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

            _logger.LogDebug("Received api AddSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api UpdateSenderNameAsync with request={requst}", JsonConvert.SerializeObject(model));

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

            _logger.LogDebug("Received api UpdateSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api DeleteSenderNameAsync with SenderNameId={SenderNameId}", SenderNameId);

            if (SenderNameId <= 0)
                return NotFound("not found");

            var response = await _senderNameService.DeleteSenderNameAsync(SenderNameId);

            _logger.LogDebug("Received api DeleteSenderNameAsync response with data={data}", JsonConvert.SerializeObject(response));

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
        public async Task<IActionResult> GetSenderNameInformationAsync(int clientId, int senderNameId)
        {
            _logger.LogDebug("Calling api GetSenderNameInformationAsync with ClientId={ClientId}, SenderNameId={SenderNameId}", clientId, senderNameId);

            if (senderNameId <= 0)
                return NotFound("not found");

            var senderName = await _senderNameService.GetSenderNameEntityByIdAsync(senderNameId);
            if (senderName == null || senderName.ClientId != clientId)
                return Ok(new ApiResult { Message = "No sender name found" });

            var response = new
            {
                ClientId = senderName.ClientId,
                SenderId = senderName.SenderId,
                SenderName = senderName.SenderName1,
                PhoneNumberId = senderName.PhoneNumberId,
                PhoneNumber = senderName.PhoneNumber,
                BusinessAccountId = senderName.BusinessAccountId,
                AccessToken = senderName.AccessToken,
                AppId = senderName.AppId,
                BusinessId = senderName.BusinessId,
                PublicCertificate = senderName.PublicCertificate,
                PrivateCertificate = senderName.PrivateCertificate
            };

            _logger.LogDebug("Received api GetSenderNameInformationAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogDebug("Calling api GetSenderNamesAsync with ClientId={ClientId}, searchStr={searchStr}", clientId, searchStr);

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
