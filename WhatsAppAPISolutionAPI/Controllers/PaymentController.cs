using Microsoft.AspNetCore.Mvc;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost("KFG")]
        public async Task<ActionResult> KFG(object payload)
        {
            var data = System.Text.Json.JsonSerializer.Serialize(payload);
            _logger.LogInformation("Calling KFG payment webhook received with data={data}", data);

            return Ok();
        }

        [HttpPost("KFGCheckPayment")]
        public async Task<ActionResult> CheckKFGPayment(object payload)
        {
            var data = System.Text.Json.JsonSerializer.Serialize(payload);
            _logger.LogInformation("Calling KFG check payment received with data={data}", data);

            return Ok();
        }
    }
}
