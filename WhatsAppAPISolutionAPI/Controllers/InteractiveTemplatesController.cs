using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.InteractiveTemplate;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class InteractiveTemplatesController : ControllerBase
    {
        private readonly ILogger<InteractiveTemplatesController> _logger;
        private readonly IInteractiveTemplateService _interactiveTemplateService;

        public InteractiveTemplatesController(ILogger<InteractiveTemplatesController> logger,
            IInteractiveTemplateService interactiveTemplateService)
        {
            _logger = logger;
            _interactiveTemplateService = interactiveTemplateService;
        }


        [HttpGet("getinteractivetemplateslist")]
        public async Task<ActionResult> GetInteractiveTemplatesListAsync(int clientId, int senderId = 0, string searchStr = "", DateTime? fromDate = null, DateTime? toDate = null, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
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
            _logger.LogInformation("Received AddInteractiveTemplateAsync request with data={data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(new ApiResult { Message = "Bad request" });

            if (string.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (model.ClientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Language))
                return Ok(new ApiResult { Message = "Please select language" });

            if (model.Body == null || String.IsNullOrEmpty(model.Body.Text))
                return Ok(new ApiResult { Message = "Body text required" });

            if (model.Buttons != null && model.Buttons.Any())
            {
                if (model.Buttons.Count() > 10)
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 2)
                    return Ok(new ApiResult { Message = "Cannot add more than 2 URL buttons." });

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

            var response = await _interactiveTemplateService.AddInteractiveTemplateAsync(model);
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

        [HttpPost("updateinteractivetemplate")]
        public async Task<IActionResult> UpdateInteractiveTemplateAsync([FromBody] InteractiveTemplateDto model)
        {
            _logger.LogInformation("Received UpdateInteractiveTemplateAsync request with data={data}", JsonConvert.SerializeObject(model));

            if (model == null)
                return Ok(new ApiResult { Message = "Bad request" });

            if (model.Id <= 0)
                return Ok(new ApiResult { Message = "Please select template" });

            if (string.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (model.ClientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Language))
                return Ok(new ApiResult { Message = "Please select language" });

            if (model.Body == null || String.IsNullOrEmpty(model.Body.Text))
                return Ok(new ApiResult { Message = "Body text required" });

            if (model.Buttons != null && model.Buttons.Any())
            {
                if (model.Buttons.Count() > 10)
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button." });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 2)
                    return Ok(new ApiResult { Message = "Cannot add more than 2 URL buttons." });

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

            var response = await _interactiveTemplateService.UpdateInteractiveTemplateAsync(model);
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

        [HttpGet("getinteractivetemplatedetail")]
        public async Task<ActionResult> GetInteractiveTemplateDetailAsync(int clientId, int senderId, int interactiveTemplateId)
        {
            var res = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, interactiveTemplateId);
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
        public async Task<IActionResult> GetAgentInteractiveTemplatesAsync(int clientId, int senderId, string language = "", string searchStr = "")
        {
            var templates = await _interactiveTemplateService.GetAgentInteractiveTemplatesAsync(clientId, senderId, language, searchStr);
            if (templates == null || !templates.Any())
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = null,
                    Message = "No records found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = templates,
                Message = String.Empty
            });
        }
    }
}