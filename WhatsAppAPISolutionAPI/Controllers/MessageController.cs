using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
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

        [HttpPost("whatsappmessagestatusupdate")]
        public async Task<IActionResult> WhatsAppMessageStatusUpdate([FromBody] WhatsAppMessageStatusUpdateDto messageStatus)
        {
            if (messageStatus == null)
            {
                return BadRequest();
            }

            var response = await _messageService.UpdateMessageStatusAsync(messageStatus);
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

        [HttpPost("whatsappmessagereceive")]
        public async Task<IActionResult> WhatsAppMessageReceive([FromBody] WhatsAppMessageStatusUpdateDto messageStatus)
        {
            if (messageStatus == null)
            {
                return BadRequest();
            }

            var response = new UResponse();
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
