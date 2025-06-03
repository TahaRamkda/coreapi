using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OrderSummaryDetailsController : ControllerBase
    {
        private readonly IOrderSummaryDetailsService _OrderSummyDetailsService;

        public OrderSummaryDetailsController(IOrderSummaryDetailsService OrderSummyDetailsService)
        {
            _OrderSummyDetailsService = OrderSummyDetailsService;
        }

        [HttpGet("OrderSummary")]
        public async Task<IActionResult> GetOrderSummaries( int clientId,  int senderId, int orderId,  int pageNo = 1,  int pageSize = 20,  string searchStr = "")
        {
            var result = await _OrderSummyDetailsService.GetOrderSummariesAsync(clientId, senderId, orderId, pageNo, pageSize, searchStr);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("OrderDetails")]
        public async Task<IActionResult> GetOrderDetails(int orderId, int pageNo = 1, int pageSize = int.MaxValue)
        {
            var result = await _OrderSummyDetailsService.GetOrderDetailsAsync(orderId, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = result,
                Message = "Data fetch successfully"
            });
        }
    }
}
