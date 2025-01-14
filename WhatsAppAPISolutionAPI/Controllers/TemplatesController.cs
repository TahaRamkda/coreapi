using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels.Agent;

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
        public async Task<ActionResult> GetTemplatesListAsync(int clientId, int transactionType = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            _logger.LogInformation("Calling api GetAgentsListAsync with clientId={clientId}, transactionType={transactionType}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, transactionType, searchStr, sortBy, pageNo, pageSize);

            var res = await _templateService.GetTemplateListAsync(clientId, transactionType, searchStr, sortBy, pageNo, pageSize);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addTemplate")]
        public async Task<IActionResult> AddTemplateAsync([FromBody] TemplateDto template)
        {
            _logger.LogInformation("Calling api AddTemplateAsync with request={requst}", JsonConvert.SerializeObject(template));

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

            if (String.IsNullOrWhiteSpace(template.Category))
                return Ok(new ApiResult
                {
                    Message = "Please select category"
                });

            if (String.IsNullOrWhiteSpace(template.Language))
                return Ok(new ApiResult
                {
                    Message = "Please select language"
                });

            if (template.Body == null || string.IsNullOrEmpty(template.Body.Text))
                return Ok(new ApiResult
                {

                    Message = "Body text required"
                });

            if (template.Buttons != null && template.Buttons.Any())
            {
                if (template.Buttons.Count() > 10)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 10 buttons."
                    });
                }

                if (template.Buttons.Count(x => x.Type == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 1 phone number button."
                    });
                }

                if (template.Buttons.Count(x => x.Type == (int)ButtonTypeEnum.URL) > 2)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 2 URL buttons."
                    });
                }
            }

            var response = await _templateService.AddTemplateAsync(template);

            _logger.LogInformation("Received api AddTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogInformation("Calling api UpdateTemplateAsync with request={requst}", JsonConvert.SerializeObject(template));

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

            if (template.Buttons != null && template.Buttons.Any())
            {
                if (template.Buttons.Count() > 10)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 10 buttons."
                    });
                }

                if (template.Buttons.Count(x => x.Type == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 1 phone number button."
                    });
                }

                if (template.Buttons.Count(x => x.Type == (int)ButtonTypeEnum.URL) > 2)
                {
                    return Ok(new ApiResult
                    {
                        Message = "Cannot add more than 2 URL buttons."
                    });
                }
            }

            var response = await _templateService.UpdateTemplateAsync(template);

            _logger.LogInformation("Received api UpdateTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogInformation("Calling api DeleteTemplateAsync with id={Id}", Id);

            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = await _templateService.DeleteTemplateAsync(Id);

            _logger.LogInformation("Received api DeleteTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

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
            _logger.LogInformation("Calling api TemplateSyncById with Id={Id}", Id);

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
            _logger.LogInformation("Calling api GetTemplateDetailsAsync with ClientId={ClientId} and Id={Id}", ClientId, Id);

            var res = await _templateService.GetTemplateDetailsAsync(ClientId, Id);

            _logger.LogInformation("Received api GetTemplateDetailsAsync response with data={data}", JsonConvert.SerializeObject(res));

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplates")]
        public async Task<IActionResult> GetTemplatesAsync(int clientId, int defaultType = 0, int senderId = 0, int transactionType = 0, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetTemplatesAsync with ClientId={ClientId}, defaultType={defaultType}, senderId={senderId}, transactionType={transactionType}, searchStr={searchStr}", clientId, defaultType, senderId, transactionType, searchStr);

            var templates = await _templateService.GetTemplatesAsync(clientId, defaultType, senderId, transactionType, searchStr);
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

        [HttpGet("gettemplatecategories")]
        public async Task<IActionResult> GetTemplateCategoriesAsync(string searchStr = "")
        {
            _logger.LogInformation("Calling api GetTemplateCategoriesAsync with searchStr={searchStr}", searchStr);

            var models = await _templateService.GetTemplateCategoriesAsync(searchStr);
            if (models == null || !models.Any())
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
                Result = models,
                Message = String.Empty
            });
        }

        [HttpGet("getlanguages")]
        public async Task<IActionResult> GetLanguagesAsync(string searchStr = "")
        {
            _logger.LogInformation("Calling api GetLanguagesAsync with searchStr={searchStr}", searchStr);

            var models = await _templateService.GetLanguagesAsync(searchStr);
            if (models == null || !models.Any())
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
                Result = models,
                Message = String.Empty
            });
        }

        [HttpGet("getdefaulttemplateslist")]
        public async Task<ActionResult> GetDefaultTemplatesListAsync(int clientId, int senderId = 0, int templateId = 0, int defaultType = 0, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetDefaultTemplatesListAsync with ClientId={ClientId}, senderId={senderId}, templateId={templateId}, defaultType={defaultType}, searchStr={searchStr}", clientId, senderId, templateId, defaultType, searchStr);

            var res = await _templateService.GetDefaultTemplateListAsync(clientId, senderId, templateId, defaultType, searchStr);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
