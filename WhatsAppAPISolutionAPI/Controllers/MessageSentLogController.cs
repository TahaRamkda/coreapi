using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly int clientId;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<MessageSentLogController> _logger;
        private readonly IMessageSentLogsService _messageSentLogsService;
        private readonly IUserService _userService;

        public MessageSentLogController(WhatsAppSolutionContext dbContext,
            ILogger<MessageSentLogController> logger,
            IMessageSentLogsService messageSentLogsService,
            IUserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _messageSentLogsService = messageSentLogsService;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getmessagesentloglist")]
        public async Task<ActionResult> GetMessageSentLogListAsync(int Id = 0, int ModuleId = 0, int ParentId = 0, string PhoneNumber = "", string WaId = "", string WaId2 = "", int SenderId = 0, DateTime? FromSentDate = null, DateTime? ToSentDate = null, DateTime? FromDeliveredDate = null, DateTime? ToDeliveredDate = null, DateTime? FromReadDate = null, DateTime? ToReadDate = null, DateTime? FromDate = null, DateTime? ToDate = null, int CurrentStatus = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogDebug("Calling api GetMessageSentLogListAsync with clientId={clientId}, Id={Id}, ModuleId={ModuleId}, ParentId={ParentId}, PhoneNumber={PhoneNumber}, WaId={WaId}, WaId2={WaId2}, SenderId={SenderId}, FromSentDate={FromSentDate}, ToSentDate={ToSentDate}, FromDeliveredDate={FromDeliveredDate}, ToDeliveredDate={ToDeliveredDate}, FromReadDate={FromReadDate}, ToReadDate={ToReadDate}, FromDate={FromDate}, ToDate={ToDate}, CurrentStatus={CurrentStatus}, SearchStr={SearchStr}, SortBy={SortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, Id, ModuleId, ParentId, PhoneNumber, WaId, WaId2, SenderId, FromSentDate, ToSentDate, FromDeliveredDate, ToDeliveredDate, FromReadDate, ToReadDate, FromDate, ToDate, CurrentStatus, SearchStr, SortBy, PageNo, PageSize);

            var res = await _messageSentLogsService.GetMessageSentLogListAsync(clientId, Id, ModuleId, ParentId, PhoneNumber, WaId, WaId2, SenderId, FromSentDate, ToSentDate, FromDeliveredDate, ToDeliveredDate, FromReadDate, ToReadDate, FromDate, ToDate, CurrentStatus, SearchStr, SortBy, PageNo, PageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
