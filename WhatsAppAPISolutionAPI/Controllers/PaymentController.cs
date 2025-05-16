using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Order.KFG.Payments;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentController(ILogger<PaymentController> logger,
            IPaymentService paymentservice)
        {
            _logger = logger;
            _paymentService = paymentservice;
        }

        //[HttpPost("KFG")]
        //public async Task<ActionResult> KFG(object payload)
        //{
        //    var data = System.Text.Json.JsonSerializer.Serialize(payload);
        //    _logger.LogInformation("Calling KFG payment webhook received with data={data}", data);

        //    return Ok();
        //}

        //[HttpPost("KFGCheckPayment")]
        //public async Task<ActionResult> CheckKFGPayment(object payload)
        //{
        //    var data = System.Text.Json.JsonSerializer.Serialize(payload);
        //    _logger.LogInformation("Calling KFG check payment received with data={data}", data);

        //    return Ok();
        //}
        [HttpPost("KFGPaymentStatusUpdate")]
        public async Task<IActionResult> KFGPaymentStatusUpdate(KFGPaymentStatus model )
        {
            _logger.LogInformation("Calling function CheckPaymentStatus with data={data}", model);

            if (model == null || string.IsNullOrEmpty(model.EncryptedKey))
                return BadRequest();

            var response = await _paymentService.CheckKFGPaymentStatusAsync(model.EncryptedKey);

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


        //[HttpPost("RecheckPaymentStatus")]
        //public async Task<IActionResult> RecheckPaymentStatus(RecheckKFGPaymentStatus model)
        //{
        //    _logger.LogInformation("Calling function RecheckKFGPaymentStatus with data={Model}", model);

        //    if (model == null || string.IsNullOrEmpty(model.tr))
        //        return BadRequest();

        //    var response = await _paymentService.CheckKFGPaymentStatusAsync(model.EncryptedKey);

        //    _logger.LogInformation("Received api WhatsAppMessageStatusUpdate response with data={data}", JsonConvert.SerializeObject(response));

        //    if (response == null || response.Status <= 0)
        //    {
        //        return Ok(new ApiResult
        //        {
        //            Result = response,
        //            Message = response?.Message
        //        });
        //    }
        //    return Ok(new ApiResult
        //    {
        //        Success = true,
        //        Result = response,
        //        Message = "Data added successfully"
        //    });
        //}
    }
}
