using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    public class MasterDataController : ControllerBase
    {
        private readonly IMasterDataService _masterDataService;
        private readonly ILogger<MasterDataController> _logger;


        public MasterDataController(IMasterDataService masterDataService,
            ILogger<MasterDataController> logger)
        {
            _masterDataService = masterDataService;
            _logger = logger;
        }

        [HttpGet("getmasterdatalist")]
        public async Task<ActionResult> GetMasterDataListAsync(string type)
        {
            _logger.LogInformation("Calling api GetMasterDataListAsync with type={type}", type);

            var res = await _masterDataService.GetMasterDataListAsync(type);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
