using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OrderSummaryController : ControllerBase
    {
        private readonly IOrderSummaryDetailsService _OrderSummyDetailsService;
        private readonly int clientId;
        private readonly IUserService _userService;

        public OrderSummaryController(IOrderSummaryDetailsService OrderSummyDetailsService, IUserService userService)
        {
            _OrderSummyDetailsService = OrderSummyDetailsService;
            _userService = userService;
            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("ordersummarylist")]
        public async Task<IActionResult> GetOrderSummaries(int senderId, int pageNo = 1,  int pageSize = 20,  string searchStr = "")
        {
            var result = await _OrderSummyDetailsService.GetOrderSummariesAsync(clientId, senderId, pageNo, pageSize, searchStr);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("ordersummarydetails")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await _OrderSummyDetailsService.GetOrderDetailsAsync(orderId);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Data fetch successfully"
            });
        }
    }
}
