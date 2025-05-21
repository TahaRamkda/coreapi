using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ReOrder;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrderController : ControllerBase
    {
        private readonly KFGOrderService _kFGOrderService;
        private readonly ILogger<MessageController> _logger;
        private readonly IOrderService _orderservice;
        private readonly IMediatorService _mediaterService;

        public OrderController(KFGOrderService kFGOrderService, IOrderService orderservice, IMediatorService mediaterService)
        {
            _kFGOrderService = kFGOrderService;
            _orderservice = orderservice;
            _mediaterService = mediaterService;
        }

        [HttpPost("RestartOrder")]
        public async Task<IActionResult> RestartOrderAsync([FromBody] ReTryOrderDto model)
        {
            _logger.LogInformation("RestartOrder called with model: {model}", JsonConvert.SerializeObject(model));
            var response = await _orderservice.RestartOrderAsync(model.ClientId, model.SenderId);

            foreach (var resp in response)
            {
                await _mediaterService.ProcessDBResponse(model.ClientId, model.SenderId, resp);
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            }
            );
        }

        [HttpPost("PushOrders")]
        public async Task<IActionResult> PushOrdersAsync(List<int> orderIds)
        {
            var response = await _orderservice.PushOrders(orderIds);
            return Ok(response);
        }
    }
}
