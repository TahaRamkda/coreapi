using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FlowsController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<FlowsController> _logger;
        private readonly IFlowsService _flowsService;
        private readonly IUserService _userService;
        private readonly FlowOpsService _flowOpsService;
        private readonly IExportManager _exportManager;

        public FlowsController(IFlowsService flowsService,
            WhatsAppSolutionContext dbContext,
            ILogger<FlowsController> logger,
            IUserService userService,
            FlowOpsService flowOpsService,
            IExportManager exportManager)
        {
            _flowsService = flowsService;
            _dbContext = dbContext;
            _logger = logger;
            _flowsService = flowsService;
            _userService = userService;
            _flowOpsService = flowOpsService;

            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
            _exportManager = exportManager;
        }

        [HttpGet("getflowslist")]
        public async Task<ActionResult> GetFlowsListAsync(string searchStr = "", int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetFlowsListAsync with clientId={clientId}, searchStr={searchStr}, pageNo={pageNo}, pageSize={pageSize}", clientId, searchStr, pageNo, pageSize);

            var res = await _flowsService.GetFlowListAsync(clientId, searchStr, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addflow")]
        public async Task<ActionResult> AddFlowAsync([FromBody] FlowDTO model)
        {
            // Log the API call with clientId and the serialized model data
            _logger.LogInformation("Calling api GetGroupByIdAsync with clientId={clientId} and object={object}", clientId, JsonConvert.SerializeObject(model));

            // Validate if the model is null
            if (model == null)
                return BadRequest();

            // Validate clientId
            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            // Validate SenderId
            if (model.SenderId <= 0)
                return Ok(new { Message = "Please enter sender id" });

            // Validate FlowName (cannot be empty or whitespace)
            if (string.IsNullOrWhiteSpace(model.FlowName))
                return Ok(new ApiResult { Message = "Please enter flow name" });

            // Validate FlowLanguage (cannot be empty or whitespace)
            if (string.IsNullOrWhiteSpace(model.FlowLanguage))
                return Ok(new ApiResult { Message = "Please select language" });

            // Restrict the number of FlowScreens to a maximum of 10
            if (model.FlowScreens.Count > 10)
                return Ok(new ApiResult { Message = "You cannot add more than 10 screens." });

            // Flattening hierarchy for better performance: extracting FlowChildren and FlowOptions
            var allChildren = model.FlowScreens.SelectMany(screen => screen.FlowChildren).ToList();
            var allOptions = allChildren.SelectMany(child => child.FlowOptions).ToList();

            // Validate FlowChildren
            foreach (var child in allChildren)
            {
                // Ensure Text property is not empty
                if (string.IsNullOrWhiteSpace(child.Text))
                    return Ok(new ApiResult { Message = "FlowChildren text cannot be empty." });

                // Restrict text length based on FlowControlType (TextInput or TextArea: max 20 characters)
                if ((child.Type == (int)FlowControlType.TextInput || child.Type == (int)FlowControlType.TextArea) && child.Text.Length > 20)
                    return Ok(new ApiResult { Message = "FlowChildren text length cannot exceed 20 characters." });

                // General restriction on text length (max 80 characters)
                if (child.Text.Length > 80)
                    return Ok(new ApiResult { Message = "FlowChildren text length cannot exceed 80 characters." });

                // Ensure at least one FlowOption exists when Type is RadioButtonsGroup (3) or CheckboxGroup (4)
                if ((child.Type == (int)FlowControlType.RadioButtonsGroup || child.Type == (int)FlowControlType.CheckboxGroup) && !child.FlowOptions.Any())
                    return Ok(new ApiResult { Message = "At least one FlowOption must be entered when FlowChildren Type is RadioButtonsGroup or CheckboxGroup." });
            }

            // Validate FlowOptions
            // Ensure OptionText is not empty
            if (allOptions.Any(option => string.IsNullOrWhiteSpace(option.OptionText)))
                return Ok(new ApiResult { Message = "FlowOption text cannot be empty." });

            // Restrict FlowOption text length to a maximum of 30 characters
            if (allOptions.Any(option => option.OptionText.Length > 30))
                return Ok(new ApiResult { Message = "FlowOption text length cannot exceed 30 characters." });

            // Call the service layer to add the Flow data asynchronously
            var response = await _flowsService.AddFlowAsync(clientId, userId, model);

            // Log the response received from the service
            _logger.LogInformation("Received api CreateFlows response with data={data}", JsonConvert.SerializeObject(response));

            // Check if the response is null or indicates failure (Status <= 0)
            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            // Return success message if data is added successfully
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPut("updateflow")]
        public async Task<ActionResult> UpdateFlowAsync([FromBody] FlowDTO model)
        {
            _logger.LogInformation("Calling api UpdateFlowAsync with clientId={clientId} and object={object}", clientId, JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (clientId <= 0)
                return Ok(new { Message = "Please enter client id" });

            if (model.SenderId <= 0)
                return Ok(new { Message = "Please enter sender id" });

            if (string.IsNullOrWhiteSpace(model.FlowName))
                return Ok(new ApiResult { Message = "Please enter flow name" });

            if (string.IsNullOrWhiteSpace(model.FlowLanguage))
                return Ok(new ApiResult { Message = "Please select language" });

            var existingFlow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == model.FlowId);
            if (existingFlow == null)
                return Ok(new ApiResult { Message = "Flow not found." });

            if (model.FlowScreens.Count > 10)
                return Ok(new ApiResult { Message = "You cannot have more than 10 screens." });

            var allChildren = model.FlowScreens.SelectMany(screen => screen.FlowChildren).ToList();
            var allOptions = allChildren.SelectMany(child => child.FlowOptions).ToList();

            foreach (var child in allChildren)
            {
                if (string.IsNullOrWhiteSpace(child.Text))
                    return Ok(new ApiResult { Message = "FlowChildren text cannot be empty." });

                if ((child.Type == (int)FlowControlType.TextInput || child.Type == (int)FlowControlType.TextArea) && child.Text.Length > 20)
                    return Ok(new ApiResult { Message = "FlowChildren text length cannot exceed 20 characters." });

                if (child.Text.Length > 80)
                    return Ok(new ApiResult { Message = "FlowChildren text length cannot exceed 80 characters." });

                if ((child.Type == (int)FlowControlType.RadioButtonsGroup || child.Type == (int)FlowControlType.CheckboxGroup) && !child.FlowOptions.Any())
                    return Ok(new ApiResult { Message = "At least one FlowOption is required for RadioButtonsGroup or CheckboxGroup." });
            }

            if (allOptions.Any(option => string.IsNullOrWhiteSpace(option.OptionText)))
                return Ok(new ApiResult { Message = "FlowOption text cannot be empty." });

            if (allOptions.Any(option => option.OptionText.Length > 30))
                return Ok(new ApiResult { Message = "FlowOption text length cannot exceed 30 characters." });

            var response = await _flowsService.UpdateFlowAsync(clientId, userId, model);
            _logger.LogInformation("Received api UpdateFlow response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpPost("publishflow")]
        public async Task<ActionResult> PublishFlowAsync(int flowId)
        {
            _logger.LogInformation("Calling api PublishFlowAsync with clientId={clientId} and flowId={flowId}", clientId, flowId);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (flowId <= 0)
                return Ok(new ApiResult { Message = "Please enter flow id" });

            // Fetch the flow by ID
            var flow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == flowId && f.ClientId == clientId);
            if (flow == null)
                return Ok(new ApiResult { Message = "Flow not found" });

            if (string.IsNullOrEmpty(flow.MetaFlowId))
                return Ok(new ApiResult { Message = "MetaFlowId not found for publish" });

            var response = await _flowsService.PublishFlowAsync(clientId, flowId);

            _logger.LogInformation("Received api PublishFlowAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpGet("getflowjsonbyid")]
        public async Task<IActionResult> GetFlowJsonByIdAsync(int flowId)
        {
            _logger.LogInformation("Calling api GetFlowJsonByIdAsync with clientId={clientId} and flowId={flowId}", clientId, flowId);

            // Fetch the flow by ID and ClientId
            var flow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == flowId && f.ClientId == clientId);
            if (flow == null)
                return NotFound(new { Message = "Flow not found" });

            var flowJson = await _flowOpsService.PrepareFlowJson(flowId);

            _logger.LogInformation("Received api GetFlowJsonByIdAsync with data={data}", flowJson);

            return Ok(flowJson); // Returning JSON response
        }

        [HttpDelete("deleteflow")]
        public async Task<IActionResult> DeleteFlowAsync(int flowId)
        {
            _logger.LogInformation("Calling api DeleteFlowAsync with id={Id}", flowId);

            if (flowId <= 0)
                return Ok(new ApiResult { Message = "Please select flow" });

            var response = await _flowsService.DeleteFlowAsync(flowId);

            _logger.LogInformation("Received api DeleteFlowAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }

        [HttpGet("getflowdetailsbyid")]
        public async Task<IActionResult> GetFlowDetailsByIdAsync(int flowId)
        {
            _logger.LogInformation("Calling api GetFlowDetailsByIdAsync with id={Id}", flowId);

            // Fetch the flow by ID and ClientId
            var flow = await _dbContext.Flows.FirstOrDefaultAsync(f => f.FlowId == flowId && f.ClientId == clientId);
            if (flow == null)
                return NotFound(new { Message = "Flow not found" });

            var flowJson = await _flowsService.GetFlowDetailsByIdAsync(flowId);

            _logger.LogInformation("Received api GetFlowDetailsByIdAsync response with data={data}", JsonConvert.SerializeObject(flowJson));

            return Ok(flowJson); // Returning JSON response
        }

        [HttpGet("getflows")]
        public async Task<IActionResult> GetFlowAsync(int senderId = 0, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetFlowAsync with ClientId={ClientId}, senderId={senderId}, searchStr={searchStr}", clientId, senderId, searchStr);

            var models = await _flowsService.GetFlowsAsync(clientId, senderId, searchStr);
            if (models == null || !models.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = models,
                Message = String.Empty
            });
        }

        [HttpGet("exportsurveyresponse")]
        public async Task<ActionResult> ExportSurveyResponseListAsync(string searchStr = "", int senderId = 0, DateTime? fromDate = null, DateTime? toDate = null, int flowId = 0, int surveyId = 0)
        {
            _logger.LogInformation("Calling api ExportSurveyResponseListAsync with clientId={clientId}, searchStr={searchStr}, senderId={senderId}, fromDate={fromDate}, toDate={toDate}, flowId={flowId}, surveyId={surveyId}", clientId, searchStr, senderId, fromDate, toDate, flowId, surveyId);

            var res = await _flowsService.ExportSurveyResponseListAsync(clientId, searchStr, senderId, fromDate, toDate, flowId, surveyId);
            var bytes = _exportManager.ExportSurveyResponseToXlsx(res);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SurveyResponse.xlsx");
        }
    }
}