using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Bridge;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Dto.Message;
using WhatsAppAPISolutionDL.Dto.Order;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using static WhatsAppAPISolutionDL.Dto.Order.KFG.KFGOrderPostingResult;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = AuthenticationSchemes.ApiKeyPolicy)]
    public class BridgeController : ControllerBase
    {
        #region Fields
        private readonly IMessageService _messageService;
        private readonly ITemplateService _templateService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<BridgeController> _logger;
        private readonly IFlowsService _flowService;
        private readonly IOrderService _orderService;
        private readonly IUserPreferenceService _userPreferenceService;
        #endregion

        #region Ctor
        public BridgeController(IMessageService messageService,
            ITemplateService templateService,
            WhatsAppSolutionContext dbContext,
            ILogger<BridgeController> logger,
            IFlowsService flowService,
            IOrderService orderService,
            IUserPreferenceService userPreferenceService)
        {
            _templateService = templateService;
            _messageService = messageService;
            _dbContext = dbContext;
            _logger = logger;
            _flowService = flowService;
            _orderService = orderService;
            _userPreferenceService = userPreferenceService;
        }
        #endregion

        #region Template

        [HttpPost("templatesync")]
        public async Task<IActionResult> TemplateSync(object templateData)
        {
            try
            {
                _logger.LogInformation("Received Template Sync response from bridge with template data={data}", JsonConvert.SerializeObject(templateData));

                var data = System.Text.Json.JsonSerializer.Serialize(templateData);

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use this if your JSON is in camelCase
                    PropertyNameCaseInsensitive = true // Ignore case when matching property names
                };

                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(data, options);

                if (result != null && result.success)
                {
                    var data1 = System.Text.Json.JsonSerializer.Serialize(result.result);
                    TemplateWithParametersDto tempParam = System.Text.Json.JsonSerializer.Deserialize<TemplateWithParametersDto>(data1, options);

                    if (tempParam != null)
                    {
                        var template = await _dbContext.Templates.Where(x => x.TemplateId == tempParam.Id).FirstOrDefaultAsync();

                        if (template == null)
                        {
                            _logger.LogInformation("Received Template Sync response from bridge but template not exist in our database with id={id}", tempParam.Id);
                            return Ok(new ApiResult
                            {
                                Message = "Template id not exist"
                            });
                        }

                        var tempDto = new TemplateStatusUpdateDto
                        {
                            Id = template.Id,
                            TemplateId = tempParam.Id,
                            Status = tempParam.Status,
                            //Category = tempParam.Category,
                            ActionBy = template.UpdatedBy != null ? template.UpdatedBy.Value : 0
                        };

                        var response = await _templateService.UpdateTemplateStatusByIdAsync(tempDto);
                        if (response == null || response.Status <= 0)
                        {
                            _logger.LogError("Received Template Sync response from bridge but unable to update template status in our database with id={id} and error = {error}", tempParam.Id, JsonConvert.SerializeObject(response?.Message));
                            return Ok(new ApiResult
                            {
                                Result = response,
                                Message = response?.Message
                            });
                        }
                        _logger.LogInformation("Received Template Sync response from bridge and updated in our database with response={response}", JsonConvert.SerializeObject(response));
                        return Ok(new ApiResult
                        {
                            Success = true,
                            Result = response,
                            Message = ""//Data added successfully"
                        });
                    }
                }
                _logger.LogError("Received Template Sync response from bridge with errors = {error}", JsonConvert.SerializeObject(templateData));
                return Ok(new ApiResult
                {
                    Message = "error in fetching template"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occurred {exception} when executing function TemplateSync with item {item}", ex, JsonConvert.SerializeObject(templateData));
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Message

        [HttpPost("whatsappmessagestatusupdate")]
        public async Task<IActionResult> WhatsAppMessageStatusUpdate([FromBody] WhatsAppMessageStatusUpdateDto messageStatus)
        {
            _logger.LogInformation("Calling function WhatsAppMessageStatusUpdate with data={messageStatus}", JsonConvert.SerializeObject(messageStatus));

            if (messageStatus == null)
            {
                return BadRequest();
            }

            var response = await _messageService.UpdateMessageStatusAsync(messageStatus);

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

        [HttpPost("whatsappmessagereceive")]
        public async Task<IActionResult> WhatsAppMessageReceive([FromBody] WhatsAppMessageReceiveDto messageReceive)
        {
            _logger.LogInformation("Calling function WhatsAppMessageReceive with data={data}", JsonConvert.SerializeObject(messageReceive));

            if (messageReceive == null)
                return BadRequest();

            var response = await _messageService.AddMessageReceivedLogAsync(messageReceive);

            _logger.LogInformation("Received api WhatsAppMessageReceive response with data={data}", JsonConvert.SerializeObject(response));

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        #endregion

        #region Flow response

        [HttpPost("flowresponse")]
        public async Task<IActionResult> FlowResponseAsync([FromBody] FlowResponseDto flowResponse)
        {
            _logger.LogInformation("Calling AddFlowResponseAsync api with data={messageStatus}", JsonConvert.SerializeObject(flowResponse));

            if (flowResponse == null)
                return BadRequest();

            var response = await _flowService.FlowResponseAsync(flowResponse);

            _logger.LogInformation("Received api AddFlowResponseAsync response with data={data}", JsonConvert.SerializeObject(response));
            return Ok(response);
        }

        #endregion

        #region Order

        [HttpPost("order")]
        public async Task<IActionResult> OrderAsync([FromBody] MetaOrderRequestDto model)
        {
            _logger.LogInformation("Calling OrderAsync with data={data}", JsonConvert.SerializeObject(model));

            if (model == null)
            {
                return BadRequest();
            }

            var response = await _orderService.CreateOrdersAsync(model);

            _logger.LogInformation("Received api CreateOrdersAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (!response.Success)
                return Ok(new ApiResult { Message = response.Message });

            return Ok(new ApiResult
            {
                Success = response.Success,
                Result = response.Result,
                Message = response.Message
            });

            //var response = await _orderService.
            //You need to continue the code
            //create OrderService
            //function CreateOrder

            //required data to procedure
            //Abbas and Shabbir

            //verify entry is getting populated in database 

            return Ok();
        }

        #endregion

        #region UserPreference
        [HttpPost("userpreferenceupdate")]

        public async Task<IActionResult> Processuserpreference(UserPreferenceDto model)
        {
            _logger.LogInformation("Received api Processuserpreference response with data={data}", JsonConvert.SerializeObject(model));
            var result = await _userPreferenceService.Processuserpreference(model);
            return Ok(result);
        }
        #endregion
    }
}
