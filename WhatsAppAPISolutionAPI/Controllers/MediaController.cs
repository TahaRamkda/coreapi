using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml.Drawing;
using System.ComponentModel;
using System.Net.Http;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Setting;
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
        private readonly ILogger<MediaController> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = String.Empty;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MediaController(IMediaService mediaService,
            WhatsAppSolutionContext dbContext,
            ILogger<MediaController> logger,
          IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
          IHttpClientFactory httpClientFactory,
          IWebHostEnvironment hostingEnvironment)
        {
            _mediaService = mediaService;
            _dbContext = dbContext;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;

            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
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

        //[HttpPost("addmedia")]
        //public async Task<IActionResult> AddMediaAsync([FromBody] MediaUploadDto media)
        //{
        //    if (media == null)
        //    {
        //        return BadRequest();
        //    }

        //    var response = await _mediaService.AddMediaAsync(media);
        //    if (response == null || response.Status <= 0)
        //    {
        //        return Ok(new ApiResult
        //        {
        //            Success = false,
        //            Result = response,
        //            Message = response?.Message
        //        });
        //    }
        //    return Ok(new ApiResult()
        //    {
        //        Success = true,
        //        Result = response,
        //        Message = "Data added successfully"
        //    });
        //}

        //[HttpPut("updatemedia")]
        //public async Task<IActionResult> UpdateMediaAsync([FromBody] MediaUploadDto media)
        //{
        //    if (media == null)
        //    {
        //        return BadRequest();
        //    }

        //    var response = await _mediaService.UpdateMediaAsync(media);
        //    if (response == null || response.Status <= 0)
        //    {
        //        return Ok(new ApiResult
        //        {
        //            Success = false,
        //            Result = response,
        //            Message = response?.Message
        //        });
        //    }
        //    return Ok(new ApiResult()
        //    {
        //        Success = true,
        //        Result = response,
        //        Message = "Data updated successfully"
        //    });
        //}

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
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }
    }
}
