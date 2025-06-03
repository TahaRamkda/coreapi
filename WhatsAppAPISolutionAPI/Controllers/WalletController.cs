using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {

        private readonly IWalleteCheckBalanceService _walletService;

        public WalletController(IWalleteCheckBalanceService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("checkBalance")]
        public async Task<IActionResult> GetBalance(int clientId)
        {
            var result = await _walletService.GetWalletBalanceAsync(clientId);
            if (result == null)
                return NotFound("Wallet info not found.");

            return Ok(result);
        }
    }
}
