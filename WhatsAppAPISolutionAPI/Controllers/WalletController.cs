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
        private readonly int ClientId;
        private readonly IUserService _userService;

        public WalletController(IWalleteCheckBalanceService walletService, IUserService userService)
        {
            _walletService = walletService;
            _userService = userService;
            ClientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("checkBalance")]
        public async Task<IActionResult> GetBalance()
        {
            var result = await _walletService.GetWalletBalanceAsync(ClientId);
            if (result == null)
                return NotFound("Wallet info not found.");

            return Ok(result);
        }
    }
}
