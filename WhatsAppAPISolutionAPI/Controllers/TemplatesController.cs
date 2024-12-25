using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<TemplatesController> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = String.Empty;

        public TemplatesController(ITemplateService templateService,
            WhatsAppSolutionContext dbContext,
            ILogger<TemplatesController> logger,
            IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
            IHttpClientFactory httpClientFactory)
        {
            _templateService = templateService;
            _dbContext = dbContext;
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;
        }

        [HttpGet("gettemplateslist")]
        public async Task<ActionResult> GetTemplatesListAsync(int ClientId, int TransactionType)
        {
            var res = await _templateService.GetTemplateListAsync(ClientId, TransactionType);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplatebyid")]
        public ActionResult GetTemplateByIdAsync(int Id)
        {
            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Templates.Where(x => x.Id == Id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult
                {
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addTemplate")]
        public async Task<IActionResult> AddTemplateAsync([FromBody] TemplateDto template)
        {
            if (template == null)
                return BadRequest();

            if (string.IsNullOrEmpty(template.Name))
                return Ok(new ApiResult
                {

                    Message = "Please insert template name"
                });

            if (template.ClientId <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert client Id"
                });

            if (template.SenderNameId <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert sender Id"
                });

            if (template.TransactionType <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert transaction type"
                });

            if (template.Body == null || string.IsNullOrEmpty(template.Body.Text))
                return Ok(new ApiResult
                {

                    Message = "Body text required"
                });

            var response = await _templateService.AddTemplateAsync(template);
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

        [HttpPut("updatetemplate")]
        public async Task<IActionResult> UpdateTemplateAsync(TemplateDto template)
        {
            if (template == null)
                return BadRequest();

            if (string.IsNullOrEmpty(template.Name))
                return Ok(new ApiResult
                {

                    Message = "Please insert template name"
                });

            if (template.ClientId <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert client Id"
                });

            if (template.SenderNameId <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert sender Id"
                });

            if (template.TransactionType <= 0)
                return Ok(new ApiResult
                {

                    Message = "Please insert transaction type"
                });

            if (template.Body == null && string.IsNullOrEmpty(template.Body.Text))
                return Ok(new ApiResult
                {

                    Message = "Body text required"
                });
             
            var response = await _templateService.UpdateTemplateAsync(template);
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
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletetemplate")]
        public async Task<IActionResult> DeleteTemplateAsync(int Id)
        {
            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _templateService.DeleteTemplateAsync(Id);
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
                Message = "Data deleted successfully"
            });
        }

        [AllowAnonymous]
        [HttpGet("templatesyncbyid")]
        public async Task<IActionResult> TemplateSyncById(int Id)
        {
            try
            {
                if (Id <= 0)
                {
                    return NotFound("not found");
                }

                var template = _dbContext.Templates.Where(x => x.Id == Id).FirstOrDefault();
                if (template == null)
                {
                    return Ok(new ApiResult
                    {

                        Message = "Incorrect template id"
                    });
                }

                _logger.LogInformation($"Input json: {JsonConvert.SerializeObject(template.TemplateId)}");

                var fullUrl = String.Concat(baseUrl, $"//api/template/synctemplatebyid?messageTemplateId={template.TemplateId}");
                var url = $"/api/template/synctemplatebyid?clientId={template.ClientId}&messageTemplateId={template.TemplateId}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResult>(content);
                if (result != null)
                {
                    if (result.success)
                    {
                        return Ok(new ApiResult
                        {
                            Success = true,
                            Message = "Template sync successfully"
                        });
                    }
                    else
                    {
                        return Ok(new ApiResult
                        {

                            Message = "Error in syncing template"
                        });
                    }
                }
                return Ok(new ApiResult
                {

                    Message = "Template not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gettemplatedetails")]
        public async Task<ActionResult> GetTemplateDetailsAsync(int ClientId, int Id = 0)
        {
            var res = await _templateService.GetTemplateDetailsAsync(ClientId, Id);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
