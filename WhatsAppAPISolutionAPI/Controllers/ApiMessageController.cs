using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<MessageController> _logger;
        private readonly IAPIMessageService _aPIMessageService;

        public ApiMessageController(WhatsAppSolutionContext dbContext,
            ILogger<MessageController> logger,
            IAPIMessageService aPIMessageService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _aPIMessageService = aPIMessageService;
        }

        [HttpGet("getapimessagelist")]
        public async Task<ActionResult> GetApiMessageListAsync(int clientId, int aPIMessageId = 0, int templateId = 0, int status = 0, string waID = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "", string trxType = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _aPIMessageService.GetAPIMessageListAsync(clientId, aPIMessageId, templateId, status, waID, fromDate, toDate, searchStr, trxType, sortBy, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
