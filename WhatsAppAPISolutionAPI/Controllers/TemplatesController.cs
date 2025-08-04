using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Text;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Carousel;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly ITemplateService _templateService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<TemplatesController> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = String.Empty;
        private readonly IUserService _userService;
        private readonly IFlowsService _flowsService;
        private readonly IInteractiveTemplateService _interactiveTemplateService;

        public TemplatesController(ITemplateService templateService,
            IFlowsService flowsService,
            IInteractiveTemplateService interactiveTemplateService,
            WhatsAppSolutionContext dbContext,
            ILogger<TemplatesController> logger,
            IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
            IHttpClientFactory httpClientFactory,
            IUserService userService)
        {
            _flowsService = flowsService;
            _interactiveTemplateService = interactiveTemplateService;
            _templateService = templateService;
            _dbContext = dbContext;
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;
            _userService = userService;



            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("gettemplateslist")]
        public async Task<ActionResult> GetTemplatesListAsync(string searchStr = "", int senderId = 0, int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue, string lang = "", string category = "")
        {
            _logger.LogDebug("Calling api GetAgentsListAsync with clientId={clientId}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}, lang={lang}", clientId, searchStr, sortBy, pageNo, pageSize, lang);

            var res = await _templateService.GetTemplateListAsync(clientId, senderId, searchStr, sortBy, pageNo, pageSize, lang, category);
            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addTemplate")]
        public async Task<IActionResult> AddTemplateAsync([FromBody] TemplateDto model)
        {
            _logger.LogDebug("Calling api AddTemplateAsync with request {request}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (String.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Category))
                return Ok(new ApiResult { Message = "Please select category" });

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
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons" });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button" });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 2)
                    return Ok(new ApiResult { Message = "Cannot add more than 2 URL buttons" });

                if (model.Buttons.Any(x => String.IsNullOrWhiteSpace(x.ButtonText)))
                    return Ok(new ApiResult { Message = "Please insert button text for all buttons" });

                if (model.Buttons.Any(x => (x.ButtonText.Length > 25)))
                    return Ok(new ApiResult { Message = "Button text should not exceed 20 characters" });
 
                if (model.Buttons.Any(x => (x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) && String.IsNullOrWhiteSpace(x.ButtonValue)))
                    return Ok(new ApiResult { Message = "Please insert button values for all URL and Phone number buttons" });

                if (model.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW) && model.Buttons.Count > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 button in case of flows" });

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

                var sameButtonParams = model.Buttons.Where(x => x.ButtonType == (int)ButtonTypeEnum.URL && x.DynamicValue != null).
                                        Select(x => x.DynamicValue.ParamName).ToList().GroupBy(x => x)
                                        .Any(g => g.Count() > 1);

                if (sameButtonParams)
                    return Ok(new ApiResult { Message = "Buttons cannot have same parameter name" });
            }

            var response = await _templateService.AddTemplateAsync(clientId, userId, model);
            _logger.LogDebug("Received api AddTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [AllowAnonymous]
        [HttpPost("carouselTemplate")]
        public async Task<IActionResult> CarouselTemplate([FromBody] CreateCarouselTemplateRequestDto model)
        {
            _logger.LogDebug("Calling api AddTemplateAsync with request {request}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });
            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (string.IsNullOrWhiteSpace(model.Category))
                return Ok(new ApiResult { Message = "Please select category" });

            if (model.Body == null || string.IsNullOrWhiteSpace(model.Body.Text))
                return Ok(new ApiResult { Message = "Body text required" });

            if (string.IsNullOrWhiteSpace(model.Language))
                return Ok(new ApiResult { Message = "Please select language" });

            StringBuilder MainHeaderMessageContent = new StringBuilder();
            MainHeaderMessageContent.AppendLine(model.Body.Text);

            if (MainHeaderMessageContent.Length > 900)
                return Ok(new ApiResult { Message = "Message content should not exceed 900 characters" });

            if (model.Cards == null || !model.Cards.Any())
                return Ok(new ApiResult { Message = "At least one card is required" });

            for (int cardIndex = 0; cardIndex < model.Cards.Count; cardIndex++)
            {
                var card = model.Cards[cardIndex];

                if (card == null)
                    return Ok(new ApiResult { Message = $"Card {cardIndex + 1} is missing" });

                if (card.Header?.Format == (int)TemplateHeaderEnum.TEXT && string.IsNullOrWhiteSpace(card.Header.Text))
                    return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Header text is required" });


                //if (card.Body == null || string.IsNullOrWhiteSpace(card.Body.Text))
                //    return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Body text is required" });

                // Validate total message content for card
                StringBuilder messageContent = new StringBuilder();

                if (card.Header?.Format == (int)TemplateHeaderEnum.TEXT)
                    messageContent.AppendLine(card.Header.Text);

                //messageContent.AppendLine(card.Body.Text);

                if (messageContent.Length > 150)
                    return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Message content should not exceed 150 characters" });

                // Validate buttons
                if (card.Buttons != null && card.Buttons.Any())
                {
                    if (card.Buttons.Count > 2)
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Cannot add more than 10 buttons" });

                    if (card.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Cannot add more than 1 phone number button" });

                    if (card.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 2)
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Cannot add more than 2 URL buttons" });

                    if (card.Buttons.Any(x => string.IsNullOrWhiteSpace(x.ButtonText)))
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Please insert button text for all buttons" });

                    if (card.Buttons.Any(x => x.ButtonText.Length > 25))
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Button text should not exceed 25 characters" });

                    if (card.Buttons.Any(x =>
                        (x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) &&
                        string.IsNullOrWhiteSpace(x.ButtonValue)))
                    {
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Button value required for URL/Phone number buttons" });
                    }

                    if (card.Buttons.Count(x => x.ActionType == (int)ActionTypeEnum.FLOW) > 1)
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Cannot add more than 1 flow button" });

                    var duplicateNames = card.Buttons
                        .GroupBy(x => x.ButtonText?.Trim())
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key)
                        .ToList();

                    if (duplicateNames.Any())
                        return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Button names must be unique" });

                    var invalidUrls = card.Buttons
                        .Where(x => x.ButtonType == (int)ButtonTypeEnum.URL && !CommonHelper.IsValidUrl(x.ButtonValue))
                        .ToList();

                    //if (invalidUrls.Any())
                    //    return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Invalid URL in buttons" });

                    var hasDuplicateParams = card.Buttons
                        .Where(x => x.ButtonType == (int)ButtonTypeEnum.URL && x.DynamicValue != null)
                        .GroupBy(x => x.DynamicValue.ParamName)
                        .Any(g => g.Count() > 1);

                    //if (hasDuplicateParams)
                    //    return Ok(new ApiResult { Message = $"Card {cardIndex + 1}: Buttons cannot have same parameter name" });
                }
            }

            var response = await _templateService.AddCarouselTemplateAsync(clientId, userId, model);
            _logger.LogDebug("Received api AddTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message ?? "Something went wrong" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }



        [HttpPost("updateTemplate")]
        public async Task<IActionResult> UpdateTemplateAsync([FromBody] TemplateDto model)
        {
            _logger.LogDebug("Calling api AddTemplateAsync with request {request}", JsonConvert.SerializeObject(model));

            if (model == null)
                return BadRequest();

            if (String.IsNullOrEmpty(model.Name))
                return Ok(new ApiResult { Message = "Please insert template name" });

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please insert client Id" });

            if (model.SenderNameId <= 0)
                return Ok(new ApiResult { Message = "Please insert sender Id" });

            if (String.IsNullOrWhiteSpace(model.Category))
                return Ok(new ApiResult { Message = "Please select category" });

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
                    return Ok(new ApiResult { Message = "Cannot add more than 10 buttons" });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 phone number button" });

                if (model.Buttons.Count(x => x.ButtonType == (int)ButtonTypeEnum.URL) > 2)
                    return Ok(new ApiResult { Message = "Cannot add more than 2 URL buttons" });

                if (model.Buttons.Any(x => String.IsNullOrWhiteSpace(x.ButtonText)))
                    return Ok(new ApiResult { Message = "Please insert button text for all buttons" });

                if (model.Buttons.Any(x => (x.ButtonText.Length > 25)))
                    return Ok(new ApiResult { Message = "Button text should not exceed 20 characters" });

                if (model.Buttons.Any(x => (x.ButtonType == (int)ButtonTypeEnum.URL || x.ButtonType == (int)ButtonTypeEnum.PHONE_NUMBER) && String.IsNullOrWhiteSpace(x.ButtonValue)))
                    return Ok(new ApiResult { Message = "Please insert button values for all URL and Phone number buttons" });

                if (model.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW) && model.Buttons.Count > 1)
                    return Ok(new ApiResult { Message = "Cannot add more than 1 button in case of flows" });

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

                var sameButtonParams = model.Buttons.Where(x => x.ButtonType == (int)ButtonTypeEnum.URL && x.DynamicValue != null).
                                        Select(x => x.DynamicValue.ParamName).ToList().GroupBy(x => x)
                                        .Any(g => g.Count() > 1);

                if (sameButtonParams)
                    return Ok(new ApiResult { Message = "Buttons cannot have same parameter name" });
            }

            var response = await _templateService.UpdateTemplateAsync(clientId, userId, model);
            _logger.LogDebug("Received api AddTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpDelete("deletetemplate")]
        public async Task<IActionResult> DeleteTemplateAsync(int Id)
        {
            _logger.LogDebug("Calling api DeleteTemplateAsync with id={Id}", Id);

            if (Id <= 0)
                return NotFound("not found");

            var response = await _templateService.DeleteTemplateAsync(Id);

            _logger.LogDebug("Received api DeleteTemplateAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

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
            _logger.LogDebug("Calling api TemplateSyncById with Id={Id}", Id);

            try
            {
                if (Id <= 0)
                    return NotFound("not found");

                var template = _dbContext.Templates.Where(x => x.Id == Id).FirstOrDefault();
                if (template == null)
                    return Ok(new ApiResult { Message = "Incorrect template id" });

                _logger.LogDebug($"Input json: {JsonConvert.SerializeObject(template.TemplateId)}");

                var fullUrl = String.Concat(baseUrl, $"/api/template/synctemplatebyid?messageTemplateId={template.TemplateId}");
                var url = $"/api/template/synctemplatebyid?clientId={template.ClientId}&senderId={template.SenderId}&messageTemplateId={template.TemplateId}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResult>(content);
                if (result != null)
                {
                    if (result.success)
                        return Ok(new ApiResult
                        {
                            Success = true,
                            Message = "Template sync successfully"
                        });
                    else
                        return Ok(new ApiResult { Message = "Error in syncing template" });
                }
                return Ok(new ApiResult { Message = "Template not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gettemplatedetails")]
        public async Task<ActionResult> GetTemplateDetailsAsync(int Id = 0)
        {
            _logger.LogDebug("Calling api GetTemplateDetailsAsync with ClientId={ClientId} and Id={Id}", clientId, Id);

            var response = await _templateService.GetTemplateDetailAsync(clientId, Id);

            _logger.LogDebug("Received api GetTemplateDetailsAsync response with data={data}", JsonConvert.SerializeObject(response));

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("gettemplates")]
        public async Task<IActionResult> GetTemplatesAsync(int senderId = 0, string searchStr = "", string category = "")
        {
            _logger.LogDebug("Calling api GetTemplatesAsync with ClientId={ClientId}, senderId={senderId}, searchStr={searchStr}, category={category}", clientId, senderId, searchStr, category);

            var templates = await _templateService.GetTemplatesAsync(clientId, senderId, searchStr, category);
            if (templates == null || !templates.Any())
                return Ok(new ApiResult { Message = "No records found" });

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
            _logger.LogDebug("Calling api GetTemplateCategoriesAsync with searchStr={searchStr}", searchStr);

            var models = await _templateService.GetTemplateCategoriesAsync(searchStr);
            if (models == null || !models.Any())
                return Ok(new ApiResult { Message = "No records found" });

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
            _logger.LogDebug("Calling api GetLanguagesAsync with searchStr={searchStr}", searchStr);

            var models = await _templateService.GetLanguagesAsync(searchStr);
            if (models == null || !models.Any())
                return Ok(new ApiResult { Message = "No records found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = models,
                Message = String.Empty
            });
        }

        #region GetTemplateVisualization
        [HttpGet("GetTemplateVisualization")]
        public async Task<ActionResult> GetFlowVisualization(int templateId, int templateType)
        {
            _logger.LogDebug("Fetching Template visualization for TemplateID={templateId}, Type={templateType}", templateId, templateType);

            if (templateId <= 0)
                return BadRequest(new { Message = "Invalid template ID" });

            if (templateType != 1 && templateType != 2)
                return BadRequest(new { Message = "Invalid template type (1=Marketing, 2=Interactive)" });

            int senderId = 456; // Example sender ID

            var result = await ProcessTemplatesRecursively(templateId, (TemplateVisualizationEnum)templateType, senderId);

            return Ok(new { Success = true, Data = result });
        }

        private async Task<List<dynamic>> ProcessTemplatesRecursively(int templateId, TemplateVisualizationEnum templateType, int senderId)
        {
            _logger.LogDebug("Fetching Template visualization for TemplateID={templateId}, Type={templateType}, SenderId = {SenderID}", templateId, templateType, senderId);


            List<dynamic> templatesList = new List<dynamic>();

            dynamic templateDetails;

            try
            {
                if (templateType == TemplateVisualizationEnum.MarketingTemplate) // Marketing Template
                {
                    var templateData = await _templateService.GetTemplateDetailAsync(clientId, templateId);
                    if (templateData == null) return templatesList;
                    templateDetails = templateData;
                }
                else if (templateType == TemplateVisualizationEnum.InteractiveTemplate) // Interactive Template
                {
                    var interactiveTemplateData = await _interactiveTemplateService.GetInteractiveTemplateDetailsAsync(clientId, senderId, templateId);
                    if (interactiveTemplateData == null) return templatesList;
                    templateDetails = interactiveTemplateData;
                }
                else if (templateType == TemplateVisualizationEnum.Flow) // Flow Template
                {
                    var flowDto = await _flowsService.GetFlowDetailsByIdAsync(templateId);
                    if (flowDto == null) return templatesList;

                    // Only select the required non-null fields
                    templateDetails = new
                    {
                        flowId = flowDto.FlowId,
                        flowName = flowDto.FlowName,
                        actionId = flowDto.ActionId,
                        actionType = flowDto.ActionType
                    };
                }
                else
                {
                    return templatesList;
                }

                // Add current template
                templatesList.Add(new
                {
                    Id = templateId,
                    Type = templateType,
                    Details = templateDetails
                });

                // Recursively process buttons
                var buttons = templateDetails.Buttons as IEnumerable<dynamic>;

                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        var actionId = (int)button.ActionId; // or string if ActionId is string

                        if ((ActionTypeEnum)button.ActionType == ActionTypeEnum.TEMPLATE) // 1 → TEMPLATE
                        {
                            _logger.LogDebug("Processing TEMPLATE type button with  SenderId={SenderId}, ActionId = {button.ActionId}", actionId, senderId);

                            var childTemplates = await ProcessTemplatesRecursively(button.ActionId, TemplateVisualizationEnum.InteractiveTemplate, senderId);
                            templatesList.AddRange(childTemplates);
                        }
                        else if ((ActionTypeEnum)button.ActionType == ActionTypeEnum.FLOW) // 8 → FLOW
                        {
                            _logger.LogDebug("Processing FLOW type button with ActionId={ActionId}, SenderId={SenderId}", actionId, senderId);

                            var flowTemplates = await ProcessTemplatesRecursively(button.ActionId, TemplateVisualizationEnum.Flow, senderId);
                            templatesList.AddRange(flowTemplates);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing templateId={TemplateId}, Type={TemplateType}", templateId, templateType);
            }

            return templatesList;
        }
        #endregion
    }
}
