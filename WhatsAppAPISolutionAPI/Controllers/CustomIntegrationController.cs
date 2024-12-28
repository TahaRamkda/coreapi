using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
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

        [HttpGet("sendsms")]
        public async Task<IActionResult> GetSendSmsAsync([FromQuery] SendSmsDto sendSms)
        {
            return await ProcessSmsRequest(sendSms);
        }

        [HttpPost("sendsms")]
        public async Task<IActionResult> PostSendSmsAsync([FromBody] SendSmsDto sendSms)
        {
            string request = JsonConvert.SerializeObject(sendSms);
            return await ProcessSmsRequest(sendSms);
        }

        private async Task<IActionResult> ProcessSmsRequest(SendSmsDto sendSms)
        {
            // Validation checks
            if (sendSms == null)
                return Ok("error - Data required");

            if (string.IsNullOrEmpty(sendSms.PhoneNumber))
                return Ok("error - Phone number required");

            if (string.IsNullOrEmpty(sendSms.TemplateName))
                return Ok("error - Template name required");

            if (string.IsNullOrEmpty(sendSms.Username))
                return Ok("error - User name required");

            if (string.IsNullOrEmpty(sendSms.Password))
                return Ok("error - Password required");

            // User login validation
            //passing RoleId=6 for APIUsers
            var res = await _userService.Login(sendSms.Username.Trim(), sendSms.Password.Trim(), (int)MasterRoleTypeEnum.APIUser);
            if (res == null || res.Status <= 0)
                return Ok("error - Incorrect Username or Password");

            // Send SMS request
            var response = await _customIntegrationService.SendSmsAsync(sendSms, res.ClientId, (int)res.UserId);

            return Ok(response);
        }
    }
}
