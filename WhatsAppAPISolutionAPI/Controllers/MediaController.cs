using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
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
            _logger.LogInformation("Calling function UploadMediaAsync");
            if (model == null)
                return BadRequest();

            if (model.File == null && model.File.Length == 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please upload file"
                });


            if (model.ClientId == 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Client does not exist"
                });

            if (model.SenderNameId == 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Sender name does not exist"
                });

            var response = await _mediaService.UploadMediaAsync(model);
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

        [HttpGet("getmedialist")]
        public async Task<ActionResult> GetMediaListAsync(int clientId, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var res = await _mediaService.GetMediaListAsync(clientId, PageNo, PageSize);
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
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _mediaService.DeleteMediaAsync(id);
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
