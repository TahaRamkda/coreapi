using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Dto.SenderName;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SenderNamesController : ControllerBase
    {
        private readonly ISenderNameService _senderNameService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<SenderNamesController> _logger;
        private readonly IMediaService _mediaService;

        public SenderNamesController(ISenderNameService senderNameService,
            WhatsAppSolutionContext dbContext,
            ILogger<SenderNamesController> logger,
            IMediaService mediaService)
        {
            _senderNameService = senderNameService;
            _dbContext = dbContext;
            _logger = logger;
            _mediaService = mediaService;
        }

        [HttpGet("getsenderNameslist")]
        public async Task<ActionResult> GetSenderNamesListAsync(int ClientId)
        {
            var res = await _senderNameService.GetSenderNameListAsync(ClientId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsenderNamebyid")]
        public async Task<ActionResult> GetSenderNameByIdAsync(int clientId, int id)
        {
            if (id <= 0)
            {
                return Ok(new ApiResult { Message = "not found" });
            }

            var res = await _senderNameService.GetSenderNameByIdAsync(clientId, id);
            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No record found with this id"
                });
            }

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
            if (model == null)
                return BadRequest();

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    return Ok(new ApiResult
                    {
                        Message = $"Cannot upload media with file extension {extension}"
                    });
                }

                var mediaUpload = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ActionBy = model.ActionBy,
                    ClientId = model.ClientId,
                    UploadToFacebook = false,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Admin
                });

                if (mediaUpload.Status <= 0 || mediaUpload.Id <= 0)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot upload media"
                    });
                }

                model.MediaId = mediaUpload.Id;
            }

            var response = await _senderNameService.AddSenderNameAsync(model);
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

        [HttpPut("updatesenderName")]
        public async Task<IActionResult> UpdateSenderNameAsync([FromForm] SenderNameDto model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    return Ok(new ApiResult
                    {
                        Message = $"Cannot upload media with file extension {extension}"
                    });
                }

                var mediaUpload = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ActionBy = model.ActionBy,
                    ClientId = model.ClientId,
                    UploadToFacebook = false,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Admin
                });

                if (mediaUpload.Status <= 0 || mediaUpload.Id <= 0)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot upload media"
                    });
                }

                model.MediaId = mediaUpload.Id;
            }

            var response = await _senderNameService.UpdateSenderNameAsync(model);
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
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletesenderName")]
        public async Task<IActionResult> DeleteSenderNameAsync(int SenderNameId)
        {
            if (SenderNameId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _senderNameService.DeleteSenderNameAsync(SenderNameId);
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

        [AllowAnonymous]
        [HttpGet("getsendernameinformation")]
        public ActionResult GetSenderNameInformationAsync(int ClientId, int SenderNameId)
        {
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

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getsendernames")]
        public async Task<IActionResult> GetSenderNamesAsync(int clientId, string searchStr = "")
        {
            var models = await _senderNameService.GetSenderNamesAsync(clientId, searchStr);
            if (models == null || !models.Any())
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = null,
                    Message = "No records found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = models,
                Message = String.Empty
            });
        }
    }
}
