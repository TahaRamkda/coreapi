using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageSentLogController : ControllerBase
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<MessageSentLogController> _logger;
        private readonly IMessageSentLogsService _messageSentLogsService;

        public MessageSentLogController(WhatsAppSolutionContext dbContext,
            ILogger<MessageSentLogController> logger,
            IMessageSentLogsService messageSentLogsService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _messageSentLogsService = messageSentLogsService;
        }

        [HttpGet("getmessagesentloglist")]
        public async Task<ActionResult> GetMessageSentLogListAsync(int clientId, int id = 0, int moduleId = 0, int parentId = 0, string phoneNumber = "", string waId = "", string waId2 = "", int senderId = 0, DateTime? fromSentDate = null, DateTime? toSentDate = null, DateTime? fromDeliveredDate = null, DateTime? toDeliveredDate = null, DateTime? fromReadDate = null, DateTime? toReadDate = null, DateTime? fromDate = null, DateTime? toDate = null, int currentStatus = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _messageSentLogsService.GetMessageSentLogListAsync(clientId, id, moduleId, parentId, phoneNumber, waId, waId2, senderId, fromSentDate, toSentDate, fromDeliveredDate, toDeliveredDate, fromReadDate, toReadDate, fromDate, toDate, currentStatus, searchStr, sortBy, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
