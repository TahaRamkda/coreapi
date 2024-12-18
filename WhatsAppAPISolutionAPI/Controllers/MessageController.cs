using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
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

        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageRequestDto sendMessage)
        {
            if (sendMessage == null)
                return BadRequest();

            if (sendMessage.ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert client Id"
                });

            if (sendMessage.SenderId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert sender Id"
                });

            if (sendMessage.Type <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert type of message"
                });

            if (sendMessage.Type == (int)MessageTypeEnum.TEXT && string.IsNullOrEmpty(sendMessage.Message))
                return Ok(new ApiResult
                {
                    Message = "Please insert message text"
                });

            if ((sendMessage.Type == (int)MessageTypeEnum.IMAGE || sendMessage.Type == (int)MessageTypeEnum.DOCUMENT) && sendMessage.MediaId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Please insert media id"
                });

            if (!sendMessage.PhoneNumbers.Any())
                return Ok(new ApiResult
                {
                    Message = "Please insert phone number(s)"
                });

            var response = await _messageService.SendMessageAsync(sendMessage);

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

        [HttpPost("sendagentmessage")]
        public async Task<IActionResult> SendAgentMessageAsync([FromBody] SendAgentMessageRequestDto model)
        {
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

            if (String.IsNullOrWhiteSpace(model.Message) || (model.File == null || model.File.Length <= 0))
            {
                return Ok(new ApiResult
                {
                    Message = "No content provided to send message"
                });
            }

            if (model.File != null && model.File.Length > 0)
            {
                var mediaUploadResult = await _mediaService.UploadMediaAsync(new MediaFileDto
                {
                    ClientId = model.ClientId,
                    SenderNameId = model.SenderId,
                    File = model.File,
                    ActionBy = model.ActionBy
                });

                if (mediaUploadResult.Status == 0)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot upload media, Please try again"
                    });
                }

                model.MediaId = mediaUploadResult.Id;
            }

            var result = await _messageService.SendAgentMessageAsync(model);

            return Ok(result);
        }
    }
}
