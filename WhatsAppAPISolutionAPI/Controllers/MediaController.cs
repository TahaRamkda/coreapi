using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var senderName = await _dbContext.SenderNames.Where(x => x.SenderId == model.Sender_Name_Id).FirstOrDefaultAsync();
            if (senderName == null)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Sender name not exist"
                });

            if (model.File != null && model.File.Length > 0)
            {
                // Check file size (50 MB = 50 * 1024 * 1024 bytes)
                const long maxFileSize = 50 * 1024 * 1024; // 50 MB
                if (model.File.Length > maxFileSize)
                {
                    return BadRequest(new ApiResult()
                    {
                        Success = false,
                        Message = "File size must not exceed 50 MB."
                    });
                }

                var originalFileName = model.File.FileName.Replace(" ", "_");
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
                    await model.File.CopyToAsync(stream);
                }

                var fileUrl = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads", Path.GetFileName(filePath));

                // Create response details
                var media = new MediaUploadDto()
                {
                    Sender_Name_Id = model.Sender_Name_Id,
                    Client_Id = model.Client_Id,
                    File_Name = Path.GetFileName(filePath),
                    File_Size = model.File.Length,
                    File_Extension = fileExtension,
                    Content_Type = model.File.ContentType,
                    Media_Path = fileUrl,
                    ActionBy = model.ActionBy
                };
                var insMedia = await _mediaService.AddMediaAsync(media);
                if (insMedia != null && insMedia.Id > 0)
                {
                    var mediaUpload = new MediaUploadBridgeDto()
                    {
                        phoneId = senderName.PhoneNumberId
                    };
                    mediaUpload.medias.Add(new MediaUploadBridgeDto.Media()
                    {
                        id = insMedia.Id.ToString(),
                        url = fileUrl,
                    });

                    var requestStr = JsonConvert.SerializeObject(mediaUpload);

                    var response = await _httpClient.PostAsync($"/api/Upload/UploadMedia", new StringContent(requestStr, null, "application/json"));
                    var content = await response.Content.ReadAsStringAsync();

                    var result = System.Text.Json.JsonSerializer.Deserialize<SyncResult>(content);
                    if (result != null && result.success)
                    {
                        var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                        var mediaResult = JsonConvert.DeserializeObject<List<MediaResult>>(data);
                        if (mediaResult != null && mediaResult.Any())
                        {
                            if (!string.IsNullOrEmpty(mediaResult[0].mediaId))
                            {
                                var updateDto = new MediaUploadDto()
                                {
                                    Id = Convert.ToInt64(mediaResult[0].id),
                                    Media_Id = mediaResult[0].mediaId
                                };
                                var updateMedia = await _mediaService.UpdateMediaAsync(media);
                                if (updateMedia == null || updateMedia.Status <= 0)
                                {
                                    return Ok(new ApiResult
                                    {
                                        Success = false,
                                        Result = updateMedia,
                                        Message = updateMedia?.Message
                                    });
                                }
                                return Ok(new ApiResult()
                                {
                                    Success = true,
                                    Result = updateMedia,
                                    Message = "Media added successfully"
                                });
                            }
                        }
                    }
                }
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
