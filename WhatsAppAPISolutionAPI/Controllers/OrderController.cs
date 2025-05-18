using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly KFGOrderService _kFGOrderService;
        private readonly ILogger<MessageController> _logger;

        public OrderController(KFGOrderService kFGOrderService)
        {
            _kFGOrderService = kFGOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int orderId)
        {
            var json = await _kFGOrderService.CreateOrder(orderId);
            return Ok(json);
        }
    }
}
