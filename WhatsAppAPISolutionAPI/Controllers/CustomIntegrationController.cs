using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using static WhatsAppAPISolutionDL.Dto.WhatsAppMessageStatusUpdateDto;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomIntegrationController : ControllerBase
    {
        private readonly ICustomIntegrationService _customIntegrationService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CustomIntegrationController> _logger;
        private readonly IUserService _userService;

        public CustomIntegrationController(ICustomIntegrationService customIntegrationService,
            WhatsAppSolutionContext dbContext,
            ILogger<CustomIntegrationController> logger,
            IUserService userService)
        {
            _customIntegrationService = customIntegrationService;
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
        }

        //[HttpGet("sendsms")]
        //public async Task<IActionResult> SendSmsAsync([FromQuery] SendSmsDto sendSms)
        //{
        //    if (sendSms == null)
        //        return Ok("error - Data required");
        //    if (string.IsNullOrEmpty(sendSms.PhoneNumber))
        //        return Ok("error - Phone number required");
        //    if (string.IsNullOrEmpty(sendSms.BrandName))
        //        return Ok("error - Brand name required");
        //    if (string.IsNullOrEmpty(sendSms.TemplateName))
        //        return Ok("error - Template name required");
        //    if (string.IsNullOrEmpty(sendSms.Username))
        //        return Ok("error - User name required");
        //    if (string.IsNullOrEmpty(sendSms.Password))
        //        return Ok("error - Password required");

        //    var res = await _userService.Login(sendSms.Username.Trim(), sendSms.Password.Trim());
        //    if (res == null || res.Status <= 0)
        //    {
        //        return Ok("error - Incorrect Username or Password");
        //    }

        //    var response = await _customIntegrationService.SendSmsAsync(sendSms, res.ClientId, (int)res.UserId);
        //    if (response == null || response.Status <= 0)
        //        return Ok($"error - {response?.Message}");

        //    return Ok("success - Data added successfully");
        //}

        //[HttpPost("sendsms")]
        //public async Task<IActionResult> SendSmsAsync(SendSmsDto sendSms)
        //{
        //    if (sendSms == null)
        //        return Ok("error - Data required");
        //    if (string.IsNullOrEmpty(sendSms.PhoneNumber))
        //        return Ok("error - Phone number required");
        //    if (string.IsNullOrEmpty(sendSms.BrandName))
        //        return Ok("error - Brand name required");
        //    if (string.IsNullOrEmpty(sendSms.TemplateName))
        //        return Ok("error - Template name required");
        //    if (string.IsNullOrEmpty(sendSms.Username))
        //        return Ok("error - User name required");
        //    if (string.IsNullOrEmpty(sendSms.Password))
        //        return Ok("error - Password required");

        //    var res = await _userService.Login(sendSms.Username.Trim(), sendSms.Password.Trim());
        //    if (res == null || res.Status <= 0)
        //    {
        //        return Ok("error - Incorrect Username or Password");
        //    }

        //    var response = await _customIntegrationService.SendSmsAsync(sendSms, res.ClientId, (int)res.UserId);
        //    if (response == null || response.Status <= 0)
        //        return Ok($"error - {response?.Message}");

        //    return Ok("success - Data added successfully");
        //}

        [HttpGet("sendsms")]
        public async Task<IActionResult> GetSendSmsAsync([FromQuery] SendSmsDto sendSms)
        {
            string page = HttpContext.Request.QueryString.ToString();
            return await ProcessSmsRequest(sendSms);
        }

        [HttpPost("sendsms")]
        public async Task<IActionResult> PostSendSmsAsync([FromBody] SendSmsDto sendSms)
        {
            string page = JsonConvert.SerializeObject(sendSms);
            return await ProcessSmsRequest(sendSms);
        }

        private async Task<IActionResult> ProcessSmsRequest(SendSmsDto sendSms)
        {
            // Validation checks
            if (sendSms == null)
                return Ok("error - Data required");
            if (string.IsNullOrEmpty(sendSms.PhoneNumber))
                return Ok("error - Phone number required");
            if (string.IsNullOrEmpty(sendSms.BrandName))
                return Ok("error - Brand name required");
            if (string.IsNullOrEmpty(sendSms.TemplateName))
                return Ok("error - Template name required");
            if (string.IsNullOrEmpty(sendSms.Username))
                return Ok("error - User name required");
            if (string.IsNullOrEmpty(sendSms.Password))
                return Ok("error - Password required");

            // User login validation
            var res = await _userService.Login(sendSms.Username.Trim(), sendSms.Password.Trim());
            if (res == null || res.Status <= 0)
            {
                return Ok("error - Incorrect Username or Password");
            }

            // Send SMS request
            var response = await _customIntegrationService.SendSmsAsync(sendSms, res.ClientId, (int)res.UserId);
            if (response == null || response.Status <= 0)
                return Ok($"error - {response?.Message}");

            return Ok("success - Data added successfully");
        }
    }
}
