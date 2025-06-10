using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionAPI.Controllers
{
    #region Routing
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    #endregion
    public class MerchantSettingInfoController : ControllerBase
    {
        #region Fields
        private readonly IMerchantSettingService _merchantService;
        #endregion

        #region Ctor
        public MerchantSettingInfoController(IMerchantSettingService merchantService)
        {
            _merchantService = merchantService;
        }
        #endregion

        #region Methods
        [AllowAnonymous]
        [HttpGet("getMerchantDetails")]
        public async Task<IActionResult> Get(string domain)
        {
            var result = await _merchantService.GetMerchantSetting(domain);
            if (result == null)
                return NotFound("Merchant info not found");
            return Ok(new ApiResult
            {
                Result = result,
                Message = "Merchant info fetched successfully",
                Success = true,
            });
        }
        #endregion 

    }
}
