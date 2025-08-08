using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CustomIntegrationController : ControllerBase
    {
        #region Fields
        private readonly ICustomIntegrationService _customIntegrationService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CustomIntegrationController> _logger;
        private readonly IUserService _userService;
        #endregion

        #region Ctor
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
        #endregion

        #region Method
        [HttpGet("sendsms")]
        public async Task<IActionResult> GetSendSmsAsync([FromQuery] SendSmsDto sendSms)
        {
            _logger.LogInformation("Calling api GetSendSmsAsync with data={data}", JsonConvert.SerializeObject(sendSms));

            return await ProcessSmsRequest(sendSms);
        }

        [HttpPost("sendsms")]
        public async Task<IActionResult> PostSendSmsAsync([FromBody] SendSmsDto sendSms)
        {
            _logger.LogInformation("Calling api PostSendSmsAsync with data={data}", JsonConvert.SerializeObject(sendSms));

            string request = JsonConvert.SerializeObject(sendSms);
            return await ProcessSmsRequest(sendSms);
        }

        private async Task<IActionResult> ProcessSmsRequest(SendSmsDto sendSms)
        {
            // Validation checks
            if (sendSms == null)
                return Ok("error - Data required");

            _logger.LogInformation("Converting custom integration object={object}", JsonConvert.SerializeObject(sendSms));
            if (!String.IsNullOrWhiteSpace(sendSms.ParamJson))
            {
                //{"BParam1:": "0.75", "BtnParam1": "35903255", "BrandName":"Pizza Hut","TemplateName":"payment_link" }  
                var json = JsonConvert.DeserializeObject<SendSmsDto>(sendSms.ParamJson);
                if (json != null)
                {
                    if (!String.IsNullOrWhiteSpace(json.PhoneNumber))
                        sendSms.PhoneNumber = json.PhoneNumber;

                    if (!String.IsNullOrWhiteSpace(json.BrandName))
                        sendSms.BrandName = json.BrandName;

                    if (!String.IsNullOrWhiteSpace(json.TemplateName))
                        sendSms.TemplateName = json.TemplateName;

                    if (!String.IsNullOrWhiteSpace(json.HParam))
                        sendSms.HParam = json.HParam;

                    if (!String.IsNullOrWhiteSpace(json.BParam1))
                        sendSms.BParam1 = json.BParam1;

                    if (!String.IsNullOrWhiteSpace(json.BParam2))
                        sendSms.BParam2 = json.BParam2;

                    if (!String.IsNullOrWhiteSpace(json.BParam3))
                        sendSms.BParam3 = json.BParam3;

                    if (!String.IsNullOrWhiteSpace(json.BParam4))
                        sendSms.BParam4 = json.BParam4;

                    if (!String.IsNullOrWhiteSpace(json.BtnParam1))
                        sendSms.BtnParam1 = json.BtnParam1;

                    if (!String.IsNullOrWhiteSpace(json.BtnParam2))
                        sendSms.BtnParam2 = json.BtnParam2;

                    if (!String.IsNullOrWhiteSpace(json.BtnParam3))
                        sendSms.BtnParam3 = json.BtnParam3;

                    if (!String.IsNullOrWhiteSpace(json.BtnParam4))
                        sendSms.BtnParam4 = json.BtnParam4;

                    if (!String.IsNullOrWhiteSpace(json.Username))
                        sendSms.Username = json.Username;

                    if (!String.IsNullOrWhiteSpace(json.Password))
                        sendSms.Password = json.Password;
                     
                    if (!String.IsNullOrWhiteSpace(json.MediaUrl))
                        sendSms.MediaUrl = json.MediaUrl;
                    
                    //sendSms.IsForceSend = json.IsForceSend;
                }

                _logger.LogInformation("Converted custom integration object={object}", JsonConvert.SerializeObject(sendSms));
            }

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

            _logger.LogInformation("Received api SendSmsAsync response with data={data}", JsonConvert.SerializeObject(response));

            return Ok(response);
        }
        #endregion
    }
}
