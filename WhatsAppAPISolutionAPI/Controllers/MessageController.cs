using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionDL.Dto;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class MessageController : ControllerBase
    {
        [HttpPost("whatsappmessagestatusupdate")]
        public async Task<IActionResult> WhatsAppMessageStatusUpdate([FromBody] WhatsAppMessageStatusUpdateDto messageStatus)
        {
            if (messageStatus == null)
            {
                return BadRequest();
            }

            return Ok(new ApiResult()
            {
                Success = true,
                Message = "Data added successfully"
            });
        }

    }
}
