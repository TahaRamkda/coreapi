using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Media;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<MessageController> _logger;
        private readonly IMediaService _mediaService;

        public MessageController(IMessageService messageService,
            WhatsAppSolutionContext dbContext,
            ILogger<MessageController> logger,
            IMediaService mediaService)
        {
            _messageService = messageService;
            _dbContext = dbContext;
            _logger = logger;
            _mediaService = mediaService;
        }

        [HttpPost("sendagentmessage")]
        public async Task<IActionResult> SendAgentMessageAsync([FromForm] SendAgentMessageRequestDto model)
        {
            _logger.LogInformation("Calling api SendAgentMessageAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (model.ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert client Id"
                });

            if (model.SenderId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert sender Id"
                });

            if (model.ConversationId <= 0)
                return Ok(new ApiResult
                {
                    Message = "No conversation found"
                });
            if(model.AgentId <= 0 || model.AgentId == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No Agent Id found"
                });

            }

            if (String.IsNullOrWhiteSpace(model.Message) && (model.File == null || model.File.Length <= 0))
            {
                return Ok(new ApiResult
                {
                    Message = "No content provided to send message"
                });
            }

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                if (!_mediaService.CheckAllowedMediaTypeForMessage(extension))
                {
                    return Ok(new ApiResult
                    {
                        Message = "The media type is not allowed"
                    });
                }

                var mediaUploadResult = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ClientId = model.ClientId,
                    SenderNameId = model.SenderId,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Conversation,
                    ActionBy = model.ActionBy
                });

                if (mediaUploadResult.Status <= 0)
                    return Ok(new ApiResult { Message = mediaUploadResult.Message });

                var media = await _dbContext.Medias.FindAsync(mediaUploadResult.Id);

                if (media == null || String.IsNullOrWhiteSpace(media.MediaId))
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot upload media, Please try again"
                    });
                }

                model.MediaId = mediaUploadResult.Id;
            }

            var result = await _messageService.SendAgentMessageAsync(model);
            var jsonresult = result.Result;
            var deserializeData = JsonConvert.DeserializeObject<StatusUpdateModel>(jsonresult.ToString());


            _logger.LogInformation("Received api SendAgentMessageAsync response with data={data}", JsonConvert.SerializeObject(result));

            return Ok(new ApiResult
            {
                Result =
            deserializeData.MessageId,
                Message = "Success",
                Success = true
            });
        }

        [HttpPost("sendagentinteractivemessage")]
        public async Task<IActionResult> SendAgentInteractiveMessageAsync([FromForm] SendAgentInteractiveMessageRequestDto model)
        {
            _logger.LogInformation("Calling api SendAgentInteractiveMessageAsync with request={requst}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (model.ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert client Id"
                });

            if (model.SenderId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert sender Id"
                });

            if (model.ConversationId <= 0)
                return Ok(new ApiResult
                {
                    Message = "No conversation found"
                });

            if (model.InteractiveTemplateId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please select interactive template"
                });

            if (model.File != null && model.File.Length > 0)
            {
                var extension = Path.GetExtension(model.File.FileName);
                if (!_mediaService.CheckAllowedMediaType(extension))
                {
                    return Ok(new ApiResult
                    {
                        Message = "The media type is not allowed"
                    });
                }

                var mediaUploadResult = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ClientId = model.ClientId,
                    SenderNameId = model.SenderId,
                    File = model.File,
                    MediaSourceId = (int)MediaSourceEnum.Conversation,
                    ActionBy = model.ActionBy
                });

                if (mediaUploadResult.Status <= 0)
                    return Ok(new ApiResult { Message = mediaUploadResult.Message });

                var media = await _dbContext.Medias.FindAsync(mediaUploadResult.Id);

                if (media == null || String.IsNullOrWhiteSpace(media.MediaId))
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot upload media, Please try again"
                    });
                }

                model.MediaId = mediaUploadResult.Id;
            }

            var result = await _messageService.SendAgentInteractiveMessageAsync(model);

            _logger.LogInformation("Received api SendAgentInteractiveMessageAsync response with data={data}", JsonConvert.SerializeObject(result));

            return Ok(result);
        }
    }
}
