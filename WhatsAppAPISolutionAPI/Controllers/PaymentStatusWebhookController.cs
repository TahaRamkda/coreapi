using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionAPI.Controllers
{
    public class PaymentStatusWebhookController : Controller
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<PaymentStatusWebhookController> _logger;
        private readonly IKFGPaymentService _kFGPaymentService;
        public PaymentStatusWebhookController( WhatsAppSolutionContext dbContext,
            ILogger<PaymentStatusWebhookController> logger,
            IKFGPaymentService kFGPaymentService
            )
        {
            _dbContext = dbContext;
            _logger = logger;
            _kFGPaymentService = kFGPaymentService;
        }
        [HttpPost("paymentstatusupdate")]
        public async Task<IActionResult> WhatsAppMessageStatusUpdate([FromBody] PaymentStatus paymentstatus)
        {
            _logger.LogInformation("Calling function WhatsAppMessageStatusUpdate with data={messageStatus}", JsonConvert.SerializeObject(paymentstatus));

            if (paymentstatus == null)
            {
                return BadRequest();
            }

            var response = await _kFGPaymentService.CheckPaymentStatusAsync(paymentstatus);

            _logger.LogInformation("Received api WhatsAppMessageStatusUpdate response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }
    }
}
