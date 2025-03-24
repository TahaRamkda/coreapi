using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ApiMessageController : ControllerBase
    {
        private readonly int clientId;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ApiMessageController> _logger;
        private readonly IAPIMessageService _aPIMessageService;
        private readonly IUserService _userService;

        public ApiMessageController(WhatsAppSolutionContext dbContext,
            ILogger<ApiMessageController> logger,
            IAPIMessageService aPIMessageService,
            IUserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _aPIMessageService = aPIMessageService;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getapimessagelist")]
        public async Task<ActionResult> GetApiMessageListAsync(int APIMessageId = 0, int TemplateId = 0, int Status = 0, string WaID = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", string TrxType = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogDebug("Calling api GetApiMessageList with ClientId={ClientId}, APIMessageId={APIMessageId}, TemplateId={TemplateId}, Status={Status}, WaID={WaID}, FromDate={FromDate}, ToDate={ToDate}, SearchStr={SearchStr}, TrxType={TrxType}, SortBy={SortBy}, PageNo={PageNo}, PageSize={PageSize}",
            clientId, APIMessageId, TemplateId, Status, WaID, FromDate, ToDate, SearchStr, TrxType, SortBy, PageNo, PageSize);

            var res = await _aPIMessageService.GetAPIMessageListAsync(clientId, APIMessageId, TemplateId, Status, WaID, FromDate, ToDate, SearchStr, TrxType, SortBy, PageNo, PageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
