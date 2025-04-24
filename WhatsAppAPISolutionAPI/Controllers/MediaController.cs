using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly int clientId;
        private readonly IMediaService _mediaService;
        private readonly ILogger<MediaController> _logger;
        private readonly IUserService _userService;

        public MediaController(IMediaService mediaService,
            ILogger<MediaController> logger,
            IUserService userService)
        {
            _mediaService = mediaService;
            _logger = logger;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpPost("uploadmedia")]
        public async Task<ActionResult> UploadMediaAsync([FromForm] MediaFileDto model)
        {
            _logger.LogDebug("Calling api UploadMediaAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            model.ClientId = clientId;
            if (model.File == null && model.File.Length == 0)
                return Ok(new ApiResult { Message = "Please upload file" });

            if (model.ClientId == 0)
                return Ok(new ApiResult { Message = "Client does not exist" });

            if (model.SenderNameId == 0)
                return Ok(new ApiResult { Message = "Sender name does not exist" });

            //Restrict other media types
            var extension = Path.GetExtension(model.File.FileName);
            if (!_mediaService.CheckAllowedMediaType(extension))
            {
                return Ok(new ApiResult
                {
                    Message = $"Cannot upload media with file extension {extension}"
                });
            }

            model.MediaSourceId = (int)MediaSourceEnum.Admin;
            var response = await _mediaService.UploadMediaAsync(model);

            _logger.LogDebug("Received api UploadMediaAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpGet("getmedialist")]
        public async Task<ActionResult> GetMediaListAsync(int senderId = 0,string FileName = "", string contentTypeStr = "", int PageNo = 0, int PageSize = int.MaxValue, int MediaTypeId = 0)
        {
            _logger.LogDebug("Calling api GetMediaListAsync with clientId={clientId}, senderId={senderId}, contentTypeStr={contentTypeStr}, pageNo={pageNo}, pageSize={pageSize}, @MediaTypeId={@MediaTypeId}", clientId, senderId, contentTypeStr, PageNo, PageSize, MediaTypeId);

            var res = await _mediaService.GetMediaListAsync(clientId, senderId, FileName, contentTypeStr, PageNo, PageSize, MediaTypeId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpDelete("deletemedia")]
        public async Task<IActionResult> DeleteMediaAsync(int id)
        {
            _logger.LogDebug("Calling api DeleteMediaAsync with id={id}", id);

            if (id <= 0)
                return NotFound("not found");

            var response = await _mediaService.DeleteMediaAsync(id);

            _logger.LogDebug("Received api DeleteMediaAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }
    }
}
