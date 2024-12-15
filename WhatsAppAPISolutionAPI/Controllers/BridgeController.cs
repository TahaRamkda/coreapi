using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class BridgeController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ITemplateService _templateService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<BridgeController> _logger;

        public BridgeController(IMessageService messageService,
            ITemplateService templateService,
            WhatsAppSolutionContext dbContext,
            ILogger<BridgeController> logger)
        {
            _templateService = templateService;
            _messageService = messageService;
            _dbContext = dbContext;
            _logger = logger;
        }

        #region Template

        [AllowAnonymous]
        [HttpPost("templatesync")]
        public async Task<IActionResult> TemplateSync(object templateData)
        {
            try
            {
                _logger.LogInformation("Calling function TemplateSync");
                _logger.LogInformation("Recieved Template Sync response from bridge with template data={data}", JsonConvert.SerializeObject(templateData));

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
                            _logger.LogInformation("Recieved Template Sync response from bridge but template not exist in our database with id={id}", tempParam.Id);
                            return Ok(new ApiResult
                            { 
                                Message = "Template id not exist"
                            });
                        }
                        
                        var tempDto = new TemplateDto 
                        {
                            Id = template.Id,
                            TemplateId = tempParam.Id,
                            Status = tempParam.Status,
                            Category = tempParam.Category,
                            ActionBy = template.UpdatedBy != null ? template.UpdatedBy.Value : 0
                        };

                        var response = await _templateService.UpdateTemplateStatusByIdAsync(tempDto);
                        if (response == null || response.Status <= 0)
                        {
                            _logger.LogError("Recieved Template Sync response from bridge but unable to update template status in our database with id={id} and error = {error}", tempParam.Id, JsonConvert.SerializeObject(response?.Message));
                            return Ok(new ApiResult
                            { 
                                Result = response,
                                Message = response?.Message
                            });
                        }
                        _logger.LogInformation("Recieved Template Sync response from bridge and updated in our database with response={response}", JsonConvert.SerializeObject(response));
                        return Ok(new ApiResult
                        {
                            Success = true,
                            Result = response,
                            Message = ""//Data added successfully"
                        });
                    }
                }
                _logger.LogError("Recieved Template Sync response from bridge with errors = {error}", JsonConvert.SerializeObject(templateData));
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
            _logger.LogInformation("Calling function WhatsAppMessageReceive with data={messageStatus}", JsonConvert.SerializeObject(messageReceive));

            if (messageReceive == null)
                return BadRequest();

            var response = await _messageService.AddMessageReceivedLogAsync(messageReceive);

            _logger.LogInformation("Recieved Add Message response from database with response={response}", JsonConvert.SerializeObject(response));

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        #endregion
    }
}
