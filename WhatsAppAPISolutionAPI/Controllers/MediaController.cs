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
        private readonly IMediaService _mediaService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediaService mediaService,
            ILogger<MediaController> logger)
        {
            _mediaService = mediaService;
            _logger = logger;
        }

        [HttpPost("uploadmedia")]
        public async Task<ActionResult> UploadMediaAsync([FromForm] MediaFileDto model)
        {
            _logger.LogInformation("Calling api UploadMediaAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (model.File == null && model.File.Length == 0)
                return Ok(new ApiResult
                {
                    Message = "Please upload file"
                });

            if (model.ClientId == 0)
                return Ok(new ApiResult
                {
                    Message = "Client does not exist"
                });

            if (model.SenderNameId == 0)
                return Ok(new ApiResult
                {
                    Message = "Sender name does not exist"
                });

            //Restrict other media types
            var extension = Path.GetExtension(model.File.FileName);
            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".3gp", ".mp4", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx", ".pdf" };
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                return Ok(new ApiResult
                {
                    Message = $"Cannot upload media with file extension {extension}"
                });
            }

            model.MediaSourceId = (int)MediaSourceEnum.Admin;
            var response = await _mediaService.UploadMediaAsync(model);

            _logger.LogInformation("Received api UploadMediaAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpGet("getmedialist")]
        public async Task<ActionResult> GetMediaListAsync(int clientId, int senderId = 0, string contentTypeStr = "", int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetMediaListAsync with clientId={clientId}, senderId={senderId}, contentTypeStr={contentTypeStr}, pageNo={pageNo}, pageSize={pageSize}", clientId, senderId, contentTypeStr, PageNo, PageSize);

            var res = await _mediaService.GetMediaListAsync(clientId, senderId, contentTypeStr, PageNo, PageSize);
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
            _logger.LogInformation("Calling api DeleteMediaAsync with id={id}", id);

            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _mediaService.DeleteMediaAsync(id);

            _logger.LogInformation("Received api DeleteMediaAsync response with data={data}", JsonConvert.SerializeObject(response));

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
    }
}
