using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiMessageController : ControllerBase
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<ApiMessageController> _logger;
        private readonly IAPIMessageService _aPIMessageService;

        public ApiMessageController(WhatsAppSolutionContext dbContext,
            ILogger<ApiMessageController> logger,
            IAPIMessageService aPIMessageService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _aPIMessageService = aPIMessageService;
        }

        [HttpGet("getapimessagelist")]
        public async Task<ActionResult> GetApiMessageListAsync(int ClientId, int APIMessageId = 0, int TemplateId = 0, int Status = 0, string WaID = "", DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", string TrxType = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling GetApiMessageList request with ClientId={ClientId}, APIMessageId={APIMessageId}, TemplateId={TemplateId}, Status={Status}, WaID={WaID}, FromDate={FromDate}, ToDate={ToDate}, SearchStr={SearchStr}, TrxType={TrxType}, SortBy={SortBy}, PageNo={PageNo}, PageSize={PageSize}",
            ClientId, APIMessageId, TemplateId, Status, WaID, FromDate, ToDate, SearchStr, TrxType, SortBy, PageNo, PageSize);

            var res = await _aPIMessageService.GetAPIMessageListAsync(ClientId, APIMessageId, TemplateId, Status, WaID, FromDate, ToDate, SearchStr, TrxType, SortBy, PageNo, PageSize);

            _logger.LogInformation("Recieved GetApiMessageList response with data={data}", JsonConvert.SerializeObject(res));

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
