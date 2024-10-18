using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml.Export.HtmlExport.StyleCollectors.StyleContracts;
using System.Text.Json;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult> GetTemplatesListAsync()
        {
            var res = await _templateService.GetTemplateListAsync();
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplatebyid")]
        public ActionResult GetTemplateByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Templates.Where(x => x.TemplatesId == id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult()
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
            {
                return BadRequest();
            }

            var response = await _templateService.AddTemplateAsync(template);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
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
            {
                return BadRequest();
            }

            var response = await _templateService.UpdateTemplateAsync(template);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data updated successfully"
            });
        }

        [HttpDelete("deletetemplate")]
        public async Task<IActionResult> DeleteTemplateAsync(int templates_Id)
        {
            if (templates_Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _templateService.DeleteTemplateAsync(templates_Id);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data deleted successfully"
            });
        }
        [AllowAnonymous]
        [HttpPost("templatesync")]
        public async Task<IActionResult> TemplateSync(object templateData)
        {
            try
            {
                var data = System.Text.Json.JsonSerializer.Serialize(templateData);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Use this if your JSON is in camelCase
                    PropertyNameCaseInsensitive = true // Ignore case when matching property names
                };
                TemplateWithParametersDto tempParam = System.Text.Json.JsonSerializer.Deserialize<TemplateWithParametersDto>(data, options);
                if (tempParam != null)
                {
                    var response = await _templateService.AddTemplateWithParameterAsync(tempParam);
                    if (response == null || response.Status <= 0)
                    {
                        return Ok(new ApiResult
                        {
                            Success = false,
                            Result = response,
                            Message = response?.Message
                        });
                    }
                    return Ok(new ApiResult()
                    {
                        Success = true,
                        Result = response,
                        Message = ""//Data added successfully"
                    });
                }
                //_logger.LogInformation("webhook received with data={data}", data);
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "error in fetching template"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("templatesyncbyid")]
        public async Task<IActionResult> TemplateSyncById(string templateId)
        {
            try
            {
                _logger.LogInformation($"Input json: {JsonConvert.SerializeObject(templateId)}");

                var fullUrl = String.Concat(baseUrl, $"//api/template/synctemplatebyid?messageTemplateId={templateId}");
                var url = $"/api/template/synctemplatebyid?messageTemplateId={templateId}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResult>(content);
                if (result != null)
                {
                    if (result.success)
                    {
                        return Ok(new ApiResult()
                        {
                            Success = true,
                            Message = "Template sync successfully"
                        });
                    }
                    else
                    {
                        return Ok(new ApiResult()
                        {
                            Success = false,
                            Message = "Error in syncing template"
                        });
                    }
                }
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Template not found"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
