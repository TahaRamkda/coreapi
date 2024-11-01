using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;

namespace WhatsAppAPISolutionAPI.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TempController : ControllerBase
    {
        private readonly ILogger<TempController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMediaService _mediaService;

        public TempController(IWebHostEnvironment hostingEnvironment,
            ILogger<TempController> logger,
            IMediaService mediaService)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _mediaService = mediaService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                _logger.LogInformation("Calling upload file");

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Define the path to save the file
                var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                // Build the file path
                var filePath = Path.Combine(uploadsFolder, file.FileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError("ex: {ex}", ex);
            }
            return Ok();
        }

        [HttpPost("uploadmedia")]
        public async Task<ActionResult> UploadMediaAsync([FromForm] MediaUploadDto model)
        {
            _logger.LogInformation("calling function UploadMediaAsync");
            if (model == null)
                return BadRequest();
            if (model.File == null && model.File.Length == 0)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Please upload file"
                });
            if (model.Sender_Name_Id == 0)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Please insert sender name"
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
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }
    }
}
