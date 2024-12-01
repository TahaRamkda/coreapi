using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

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

        public MessageController(IMessageService messageService,
            WhatsAppSolutionContext dbContext,
            ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageRequestDto sendMessage)
        {
            if (sendMessage == null)
                return BadRequest();

            if (sendMessage.ClientId <= 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert client Id"
                });

            if (sendMessage.SenderId <= 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert sender Id"
                });

            if (sendMessage.Type <= 0)
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert type of message"
                });

            if (sendMessage.Type == (int)MessageTypeEnum.TEXT && string.IsNullOrEmpty(sendMessage.Message))
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert message text"
                });

            if (sendMessage.Type != (int)MessageTypeEnum.TEXT && string.IsNullOrEmpty(sendMessage.MediaId))
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert media id"
                });

            if (!sendMessage.PhoneNumbers.Any())
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "Please insert phone number(s)"
                });

            var response = await _messageService.SendMessageAsync(sendMessage);
            
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
    }
}
