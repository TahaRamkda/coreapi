using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult> GetMessageSentLogListAsync(int ClientId, int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var res = await _messageSentLogsService.GetMessageSentLogListAsync(ClientId, Id, ModuleId, ParentId, PhoneNumber, WaId, WaId2, SenderId, FromSentDate, ToSentDate, FromDeliveredDate, ToDeliveredDate, FromReadDate, ToReadDate, FromDate, ToDate, CurrentStatus, SearchStr, SortBy, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
