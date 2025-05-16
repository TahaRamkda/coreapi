using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Message;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentLinkController : Controller
    {
        #region Fields
        private readonly ILogger<PaymentLinkController> _logger;
        private readonly ICommunicationService _communicationService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IUserService _userService;
        private int ClientId = 0;
        private readonly string _secretKey;
        private readonly KFGPaymentConfiguration _config;
        private readonly IPaymentService _kfgPayementService;

        #endregion

        #region Ctor
        public PaymentLinkController(
            ILogger<PaymentLinkController> logger,
            ICommunicationService communicationService,
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IUserService userService, IOptions<KFGPaymentConfiguration> config,
            IPaymentService kFGPaymentService)
        {
            _logger = logger;
            _communicationService = communicationService;
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _userService = userService;
            _config = config.Value;
            _kfgPayementService = kFGPaymentService;

            ClientId = _userService.GetClientIdFromAccessToken();
            _secretKey = _config.SecretKey;
        }

        #endregion

        [HttpPost("sendpaymentlink")]
        public async Task<IActionResult> CreatePaymentLink(int orderId, decimal amount, int senderId, int clientId)
        {
            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            try
            {
                var url = await SendPaymentLink(orderId, amount);

                InteractiveMessageRequestDto requestDto;

                if (string.IsNullOrEmpty(url.ToString()))
                {
                    var paramValues = new List<ParamValue>
            {
                new ParamValue { Key = "order_id", Value = orderId.ToString() },
                new ParamValue { Key = "amount", Value = amount.ToString("0.00") }
            };

                    requestDto = new InteractiveMessageRequestDto
                    {
                        ClientId = clientId,
                        SenderId = senderId,
                        PhoneNumber = "917732862398",
                        ModuleId = (int)ModuleEnum.Order,
                        ActionId = 94,
                        ParentId = 0,
                        HeaderType = 0,
                        MediaId = 0,
                        HeaderText = "",
                        BodyText = $"Please complete the payment of ₹{amount:0.00} for Order #{orderId}.",
                        FooterText = "Click below to proceed to payment.",
                        FlowToken = "", 
                        Values = paramValues,
                        Buttons = new List<InteractiveMessageRequestDto.Button>
                {
                    new InteractiveMessageRequestDto.Button
                    {
                        ButtonText = "Pay Now",
                        ButtonValue = url.ToString(),
                        ButtonType = 3,
                        Sequence = 0,
                        ActionId = 0,
                        ActionType = 0
                    }
                }
                    };
                }
                else
                {   // Send default template with a retry button when URL is null
                    requestDto = new InteractiveMessageRequestDto
                    {
                        ClientId = clientId,
                        SenderId = senderId,
                        PhoneNumber = "917732862398",
                        ModuleId = (int)ModuleEnum.Order,
                        ActionId = 95,
                        ParentId = 0,
                        HeaderType = 0,
                        MediaId = 0,
                        HeaderText = "",
                        BodyText = $"We were unable to generate a payment link for Order #{orderId}.\nPlease contact support or try again later.",
                        FooterText = "Click below to retry.",
                        FlowToken = "", 
                        Values = new List<ParamValue>(),
                        Buttons = new List<InteractiveMessageRequestDto.Button>
                        {
                            new InteractiveMessageRequestDto.Button
                            {
                                ButtonText = "Retry Payment",
                                ButtonValue = $"retry_{orderId}_{amount}",
                                ButtonType = 3,
                                Sequence = 1,
                                ActionId = 0,
                                ActionType = 0
                            }
                        }
                    };
                }

                var response = await _communicationService.SendInteractiveMessageAsync(requestDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending payment link.");
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpPost("payment")]
        public async Task<IActionResult> SendPaymentLink(int orderId, decimal amount)
        {
            {
                // Simulate success/failure
                //if (orderId > 0 && amount > 0)
                //{
                //    var paymentUrl = $"https://demo.payments.com/pay?orderId={orderId}&amount={amount}";
                //    return Ok(new { success = true, url = paymentUrl });
                //}

                return Ok(new { success = false, url = (string?)null });
            }
        }

        [HttpPost]
        public IActionResult ReceiveKFGPaymentStatus([FromBody] WebhookPayload payload)
        {
            try
            {
                var response = _kfgPayementService.DecryptKfgResponse(payload.EncryptedKey);  
                
                //write logic to save in db

                return Ok(new { message = "Received" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}

