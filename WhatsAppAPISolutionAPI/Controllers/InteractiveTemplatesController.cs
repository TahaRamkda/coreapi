using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.InteractiveTemplate;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class InteractiveTemplatesController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly ILogger<InteractiveTemplatesController> _logger;
        private readonly IInteractiveTemplateService _interactiveTemplateService;
        private readonly IUserService _userService;

        public InteractiveTemplatesController(ILogger<InteractiveTemplatesController> logger,
            IInteractiveTemplateService interactiveTemplateService,
            IUserService userService)
        {
            _logger = logger;
            _interactiveTemplateService = interactiveTemplateService;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }


        [HttpGet("getinteractivetemplateslist")]
        public async Task<ActionResult> GetInteractiveTemplatesListAsync(int senderId = 0, string searchStr = "", DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentsListAsync with clientId={clientId}, senderId={senderId}, searchStr={searchStr}, fromDate={fromDate}, toDate={toDate}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, senderId, searchStr, fromDate, toDate, sortBy, pageNo, pageSize);

            var res = await _interactiveTemplateService.GetInteractiveTemplateListAsync(clientId, senderId, searchStr, fromDate, toDate, sortBy, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addinteractivetemplate")]
        public async Task<IActionResult> AddInteractiveTemplateAsync([FromBody] InteractiveTemplateDto model)
        {
            _logger.LogInformation("Calling api AddInteractiveTemplateAsync request with data={data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(new ApiResult { Message = "Bad request" });

            if (string.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Language))
                return Ok(new ApiResult { Message = "Please select language" });

            if (model.Header != null && model.Header.Format == (int)TemplateHeaderEnum.TEXT && String.IsNullOrEmpty(model.Header.Text))
                return Ok(new ApiResult { Message = "Header text is required" });

            if (model.Body == null || String.IsNullOrEmpty(model.Body.Text))
                return Ok(new ApiResult { Message = "Body text required" });

            StringBuilder messageContent = new StringBuilder();
            if (model.Header != null && model.Header.Format == (int)TemplateHeaderEnum.TEXT)
            {
                messageContent.Append(model.Header.Text);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(model.Body.Text))
            {
                messageContent.Append(model.Body.Text);
                messageContent.AppendLine();
            }

            if (model.Footer != null && !String.IsNullOrWhiteSpace(model.Footer.Text))
                messageContent.Append(model.Footer.Text);

            if (messageContent.ToString().Length > 900)
                return Ok(new ApiResult { Message = "Message content should not exceed 900 characters" });

            if (model.Buttons != null && model.Buttons.Any())
            {
                if (model.Buttons.Count() > 10)
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons." });

                var urlButtonExists = model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER);
                if (urlButtonExists > 0)
                {
                    if (model.Buttons.Count() > 1)
                        return Ok(new ApiResult { Message = "Cannot add more than 1 buttons if either URL or Phone Number button exists." });
                }

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 URL buttons." });

                if (model.Buttons.Any(x => String.IsNullOrWhiteSpace(x.ButtonText)))
                    return Ok(new ApiResult { Message = "Please insert button text for all buttons" });

                if (model.Buttons.Any(x => (x.ButtonText.Length > 20)))
                    return Ok(new ApiResult { Message = "Button text should not exceed 20 characters" });

                if (model.Buttons.Any(x => (x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) && String.IsNullOrWhiteSpace(x.ButtonValue)))
                    return Ok(new ApiResult { Message = "Please insert button values for all URL and Phone number buttons" });

                // Validate no duplicate button names
                var duplicateNames = model.Buttons.GroupBy(item => item.ButtonText?.Trim()).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

                if (duplicateNames.Any())
                    return Ok(new ApiResult { Message = "Button names should be unique." });

                // Validate Button URLs  
                var invalidUrls = model.Buttons
                    .Where(item => item.ButtonType == (int)ButtonTypeEnum.URL && !CommonHelper.IsValidUrl(item.ButtonValue))
                    .ToList();

                if (invalidUrls.Any())
                    return Ok(new ApiResult { Message = "Invalid url provided in buttons" });
            }

            var response = await _interactiveTemplateService.AddInteractiveTemplateAsync(clientId, userId, model);

            _logger.LogInformation("Received api AddInteractiveTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("updateinteractivetemplate")]
        public async Task<IActionResult> UpdateInteractiveTemplateAsync([FromBody] InteractiveTemplateDto model)
        {
            _logger.LogInformation("Calling api UpdateInteractiveTemplateAsync request with data={data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(new ApiResult { Message = "Bad request" });

            if (model.Id <= 0)
                return Ok(new ApiResult { Message = "Please select template" });

            if (string.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Language))
                return Ok(new ApiResult { Message = "Please select language" });

            if (model.Header != null && model.Header.Format == (int)TemplateHeaderEnum.TEXT && String.IsNullOrEmpty(model.Header.Text))
                return Ok(new ApiResult { Message = "Header text is required" });

            if (model.Body == null || String.IsNullOrEmpty(model.Body.Text))
                return Ok(new ApiResult { Message = "Body text required" });

            StringBuilder messageContent = new StringBuilder();
            if (model.Header != null && model.Header.Format == (int)TemplateHeaderEnum.TEXT)
            {
                messageContent.Append(model.Header.Text);
                messageContent.AppendLine();
            }

            if (!String.IsNullOrWhiteSpace(model.Body.Text))
            {
                messageContent.Append(model.Body.Text);
                messageContent.AppendLine();
            }

            if (model.Footer != null && !String.IsNullOrWhiteSpace(model.Footer.Text))
                messageContent.Append(model.Footer.Text);

            if (messageContent.ToString().Length > 900)
                return Ok(new ApiResult { Message = "Message content should not exceed 900 characters" });

            if (model.Buttons != null && model.Buttons.Any())
            {
                if (model.Buttons.Count > 10)
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons." });

                var urlButtonExists = model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER);
                if (urlButtonExists > 0)
                {
                    if (model.Buttons.Count > 1)
                        return Ok(new ApiResult { Message = "Cannot add more than 1 buttons if either URL or Phone Number button exists." });
                }

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 URL buttons." });

                if (model.Buttons.Any(x => String.IsNullOrWhiteSpace(x.ButtonText)))
                    return Ok(new ApiResult { Message = "Please insert button text for all buttons" });

                if (model.Buttons.Any(x => (x.ButtonText.Length > 20)))
                    return Ok(new ApiResult { Message = "Button text should not exceed 20 characters" });

                if (model.Buttons.Any(x => (x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) && String.IsNullOrWhiteSpace(x.ButtonValue)))
                    return Ok(new ApiResult { Message = "Please insert button values for all URL and Phone number buttons" });

                // Validate no duplicate button names
                var duplicateNames = model.Buttons.GroupBy(item => item.ButtonText?.Trim()).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

                if (duplicateNames.Any())
                    return Ok(new ApiResult { Message = "Button names should be unique." });

                // Validate Button URLs  
                var invalidUrls = model.Buttons
                    .Where(item => item.ButtonType == (int)ButtonTypeEnum.URL && !CommonHelper.IsValidUrl(item.ButtonValue))
                    .ToList();

                if (invalidUrls.Any())
                    return Ok(new ApiResult { Message = "Invalid url provided in buttons" });
            }

            var response = await _interactiveTemplateService.UpdateInteractiveTemplateAsync(clientId, userId, model);

            _logger.LogInformation("Received api UpdateInteractiveTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpGet("getinteractivetemplatedetail")]
        public async Task<ActionResult> GetInteractiveTemplateDetailAsync(int senderId, int interactiveTemplateId)
        {
            _logger.LogInformation("Calling api GetInteractiveTemplateDetailAsync with clientId={clientId}, senderId={senderId}, interactiveTemplateId={interactiveTemplateId}", clientId, senderId, interactiveTemplateId);

            var res = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, interactiveTemplateId);

            _logger.LogInformation("Received api GetInteractiveTemplateDetailAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "Template not found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagentinteractivetemplates")]
        public async Task<IActionResult> GetAgentInteractiveTemplatesAsync(int senderId, string language = "", string searchStr = "")
        {
            _logger.LogInformation("Calling api GetAgentInteractiveTemplatesAsync with clientId={clientId}, senderId={senderId}, language={language}, searchStr={searchStr}", clientId, senderId, language, searchStr);

            var templates = await _interactiveTemplateService.GetAgentInteractiveTemplatesAsync(clientId, senderId, language, searchStr);
            if (templates == null || !templates.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = templates,
                Message = String.Empty
            });
        }

        [HttpGet("getinteractivetemplatewithoutparams")]
        public async Task<IActionResult> GetInteractiveTemplateWithoutParamsAsync(int senderId, string language = "", string searchStr = "")
        {
            _logger.LogInformation("Calling api GetInteractiveTemplateWithoutParamsAsync with clientId={clientId}, senderId={senderId}, language={language}, searchStr={searchStr}", clientId, senderId, language, searchStr);

            var templates = await _interactiveTemplateService.GetInteractiveTemplateWithoutParamsAsync(clientId, senderId, language, searchStr);
            if (templates == null || !templates.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = templates,
                Message = String.Empty
            });
        }
    }
}