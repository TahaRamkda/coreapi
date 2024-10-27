using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class MediaController : ControllerBase
    {
        private readonly string _uploadPath;
        private readonly IMediaService _mediaService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ContactsController> _logger;

        public MediaController(IMediaService mediaService,
            WhatsAppSolutionContext dbContext,
            ILogger<ContactsController> logger)
        {
            _mediaService = mediaService;
            _dbContext = dbContext;
            _logger = logger;

            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpPost("uploadmedia")]
        public async Task<ActionResult> UploadMediaAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var originalFileName = file.FileName.Replace(" ","_");
                var filePath = Path.Combine(_uploadPath, originalFileName);

                // Check if the file already exists and create a unique filename if it does
                var fileExtension = Path.GetExtension(originalFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                int counter = 1;

                while (System.IO.File.Exists(filePath))
                {
                    var newFileName = $"{fileNameWithoutExtension}_{counter}{fileExtension}";
                    filePath = Path.Combine(_uploadPath, newFileName);
                    counter++;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create the full URL for the uploaded file
                var fileUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{Path.GetFileName(filePath)}";

                return Ok(new ApiResult()
                {
                    Success = true,
                    Result = fileUrl,
                    Message = "Media uploaded successfully"
                });
            }

            return Ok(new ApiResult()
            {
                Success = false,
                Message = "Error in uploading media"
            });
        }

        [HttpGet("getmedialist")]
        public async Task<ActionResult> GetMediaListAsync(int client_Id)
        {
            var res = await _mediaService.GetMediaListAsync(client_Id);
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addmedia")]
        public async Task<IActionResult> AddMediaAsync([FromBody] MediaUploadDto media)
        {
            if (media == null)
            {
                return BadRequest();
            }

            var response = await _mediaService.AddMediaAsync(media);
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

        [HttpPut("updatemedia")]
        public async Task<IActionResult> UpdateMediaAsync([FromBody] MediaUploadDto media)
        {
            if (media == null)
            {
                return BadRequest();
            }

            var response = await _mediaService.UpdateMediaAsync(media);
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

        [HttpDelete("deletemedia")]
        public async Task<IActionResult> DeleteMediaAsync(int media_Id)
        {
            if (media_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _mediaService.DeleteMediaAsync(media_Id);
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
