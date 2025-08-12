using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Carousel;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Template;
using static WhatsAppAPISolutionDL.Dto.Catalog.CatalogDto;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly IMediaService _mediaService;
        private readonly ILogger<TemplateService> _logger;
        private readonly ICacheService _cacheService;

        public TemplateService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IMediaService mediaService,
            ILogger<TemplateService> logger,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _mediaService = mediaService;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<List<UTemplate>> GetTemplateListAsync(int clientId, int senderId, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue, string lang = "", string cat = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Templates.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @SenderId= {senderId}, @SearchStr={searchStr},@SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}, @Language={lang}, @Category={cat}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Templates_Ops with parameters: " +
                "ActionId={ActionId}, ClientId={ClientId},SenderId= {SenderId}, SearchStr={SearchStr}, SortBy={SortBy}, PageNo={PageNo}, PageSize={PageSize}, lang={lang}, cat ={cat}, ProcResponseTime={ProcResponseTime}ms",
                (int)CrudEnum.List, clientId, senderId, searchStr, sortBy, pageNo, pageSize, lang, cat, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds); return response;
        }

        public async Task<UResponseWithID> AddTemplateAsync(int clientId, int userId, TemplateDto model)
        {
            //Replace empty space with _
            model.Name = model.Name.Replace(" ", "_").ToLower().Trim();

            //Check if template name already exists
            var templateNameExist = await _dbContext.Templates
                .Where(x => x.ClientId == clientId
                && x.SenderId == model.SenderNameId
                //&& x.RecordStatus != -1
                && x.TemplateName != null
                && x.TemplateName.ToLower() == model.Name.ToLower()
                //&& x.Language != null
                //&& x.Language.ToLower() == model.Language.ToLower()).
                ).FirstOrDefaultAsync();

            if (templateNameExist != null)
                return new UResponseWithID { Message = "Template with same name already exist" };

            //Globals 
            Regex regex = new Regex(CommonHelper.DynamicPattern);
            int headerType = 0;
            string headerText = String.Empty;
            int headerTextCount = 0;
            string bodyText = String.Empty;
            int bodyTextCount = 0;
            string footerText = String.Empty;
            List<TemplateDto.TemplateParameter> parameters = new List<TemplateDto.TemplateParameter>();
            List<TemplateDto.TemplateButton> buttons = new List<TemplateDto.TemplateButton>();

            //If header exist
            if (model.Header != null)
            {
                if (model.Header.Format < 0)
                    return new UResponseWithID { Message = "Header format not mentioned" };

                headerType = model.Header.Format;
                var headerFormat = (TemplateHeaderEnum)model.Header.Format;
                var headerMediaTypes = new List<TemplateHeaderEnum> { TemplateHeaderEnum.IMAGE, TemplateHeaderEnum.VIDEO, TemplateHeaderEnum.DOCUMENT };
                if (headerMediaTypes.Contains(headerFormat))
                {
                    //Media related validations
                    if (model.MediaId <= 0)
                        return new UResponseWithID { Message = "Media is required when header type is not text" };

                    var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID { Message = "Media not exist" };

                    if (mediaDetail.SenderNameId != model.SenderNameId)
                        return new UResponseWithID { Message = "Media does not exist for this sender" };

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType(headerFormat, mediaDetail.FileExtension);
                    if (!allowedMedia)
                        return new UResponseWithID { Message = $"Not allowed media for header type - {headerFormat}" };

                }

                if (headerFormat == TemplateHeaderEnum.TEXT)
                {
                    if (String.IsNullOrWhiteSpace(model.Header.Text))
                        return new UResponseWithID { Message = "Header text is required" };

                    model.Header.Text = model.Header.Text.Trim();
                    headerText = model.Header.Text;

                    MatchCollection matches = regex.Matches(model.Header.Text);
                    if (matches.Count > 0)
                    {
                        if (model.Header.DynamicValue == null || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamValue))
                            return new UResponseWithID { Message = "Header default parameter is required" };

                        if (matches.Count > 1)
                            return new UResponseWithID { Message = "Only one header parameter is allowed" };

                        if (matches[0].Value != model.Header.DynamicValue.ParamName)
                            return new UResponseWithID { Message = "Headere parameter passed does not match with header text" };

                        headerTextCount = matches.Count;
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Header,
                            ParamName = model.Header.DynamicValue.ParamName,
                            ParamDefaultValue = model.Header.DynamicValue.ParamValue,
                            Sequence = 0
                        });
                    }
                }
            }

            //If body exist
            if (model.Body != null)
            {
                if (String.IsNullOrWhiteSpace(model.Body.Text))
                    return new UResponseWithID { Message = "Body text is required" };

                model.Body.Text = model.Body.Text.Trim();
                bodyText = model.Body.Text;
                MatchCollection matches = regex.Matches(model.Body.Text);
                if (matches.Count > 0)
                {
                    if (model.Body.DynamicValues == null || model.Body.DynamicValues.Count == 0)
                        return new UResponseWithID { Message = "Body text parameter is required" };

                    var duplicate = matches.Select(x => x.Value).GroupBy(item => item).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                    if (duplicate.Any())
                        return new UResponseWithID { Message = "Cannot have duplicate parameters in body." };

                    if (matches.Count != model.Body.DynamicValues.Count)
                        return new UResponseWithID { Message = "Body text parameters is not matching with body text count" };

                    var passedEmpty = model.Body.DynamicValues.Any(x => String.IsNullOrWhiteSpace(x.ParamName) || String.IsNullOrWhiteSpace(x.ParamValue));
                    if (passedEmpty)
                        return new UResponseWithID { Message = "Parameter name or value is not passed correctly in body" };

                    var matchValues = matches.Select(x => x.Value).ToList();
                    var bodyParams = model.Body.DynamicValues.Select(x => x.ParamName).ToList();

                    var areEqual = matchValues.All(item => bodyParams.Contains(item));
                    if (!areEqual)
                        return new UResponseWithID { Message = "Body parameters passed does not match with body text" };

                    bodyTextCount = matches.Count;
                    for (int i = 0; i < model.Body.DynamicValues.Count; i++)
                    {
                        var value = model.Body.DynamicValues[i];
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Body,
                            ParamName = value.ParamName,
                            ParamDefaultValue = value.ParamValue,
                            Sequence = i
                        });
                    }
                }
            }

            if (model.Footer != null)
                footerText = model.Footer.Text.Trim();

            if (model.Buttons != null && model.Buttons.Count > 0)
            {
                for (int i = 0; i < model.Buttons.Count; i++)
                {
                    var button = model.Buttons[i];
                    if (button.ActionType == (int)ActionTypeEnum.TEMPLATE && button.ActionId <= 0)
                        return new UResponseWithID { Message = $"Template id required in action id when action type is {(int)ActionTypeEnum.TEMPLATE}" };

                    button.ButtonValue = (button.ButtonValue ?? "").Trim();


                    if (!String.IsNullOrWhiteSpace(button.ButtonValue))
                    {
                        MatchCollection matches = regex.Matches(button.ButtonValue);
                        if (matches.Count > 0)
                        {
                            if (button.ButtonType != (int)ButtonTypeEnum.URL)
                                return new UResponseWithID { Message = $"Dynamic parameter not allowed in button type {(ButtonTypeEnum)button.ButtonType}" };

                            if (button.DynamicValue == null || String.IsNullOrWhiteSpace(button.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(button.DynamicValue.ParamValue))
                                return new UResponseWithID { Message = "Button default parameter is required" };

                            if (matches.Count > 1)
                                return new UResponseWithID { Message = "Only one button parameter is allowed" };

                            if (matches[0].Value != button.DynamicValue.ParamName)
                                return new UResponseWithID { Message = $"Button - {button.ButtonText} parameter passed does not match with button value" };

                            parameters.Add(new TemplateDto.TemplateParameter
                            {
                                ParamType = (int)TemplateParamEnum.Button,
                                ParamName = button.DynamicValue.ParamName,
                                ParamDefaultValue = button.DynamicValue.ParamValue,
                                Sequence = button.Sequence //Assigning button sequence
                            });
                        }
                    }

                    buttons.Add(new TemplateDto.TemplateButton
                    {
                        ButtonType = button.ButtonType,
                        ButtonText = button.ButtonText,
                        ButtonValue = button.ButtonValue,
                        ActionId = button.ActionId,
                        ActionType = button.ActionType,
                        Sequence = button.Sequence,
                        SytemActionId = button.SytemActionId
                    });
                }
            }

            var parameterJson = JsonConvert.SerializeObject(parameters);
            var buttonJson = JsonConvert.SerializeObject(buttons);

            var responseList = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @SenderId={model.SenderNameId},  @TemplateName={model.Name},@Category={model.Category}, @Language={model.Language}, @HeaderType={headerType}, @HeaderParamCount={headerTextCount}, @HeaderText={headerText},@MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyTextCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={userId}").ToListAsync();

            await _cacheService.RemoveByPrefix(CacheKeys.TEMPLATE_PATTERN_KEY);

            if (responseList == null || !responseList.Any())
                return new UResponseWithID { Message = "Cannot add template" };

            var response = responseList[0];
            if (response.Status <= 0)
                return new UResponseWithID { Message = response.Message };

            //Push template to facebook
            return await PushTemplateToFacebook(clientId, response.Id);
        }

        public async Task<UResponseWithID> UpdateTemplateAsync(int clientId, int userId, TemplateDto model)
        {
            //Replace empty space with _
            model.Name = model.Name.Replace(" ", "_").ToLower().Trim();

            //Check if template name already exists
            var templateNameExist = await _dbContext.Templates
                .Where(x => x.ClientId == clientId
                && x.SenderId == model.SenderNameId
                //&& x.RecordStatus != -1
                && x.TemplateName != null
                && x.TemplateName.ToLower() == model.Name.ToLower()
                && x.Id != model.Id
                //&& x.Language != null
                //&& x.Language.ToLower() == model.Language.ToLower()).
                ).FirstOrDefaultAsync();

            if (templateNameExist != null)
                return new UResponseWithID { Message = "Template with same name already exist" };

            //Globals 
            Regex regex = new Regex(CommonHelper.DynamicPattern);
            int headerType = 0;
            string headerText = String.Empty;
            int headerTextCount = 0;
            string bodyText = String.Empty;
            int bodyTextCount = 0;
            string footerText = String.Empty;
            List<TemplateDto.TemplateParameter> parameters = new List<TemplateDto.TemplateParameter>();
            List<TemplateDto.TemplateButton> buttons = new List<TemplateDto.TemplateButton>();

            //If header exist
            if (model.Header != null)
            {
                if (model.Header.Format < 0)
                    return new UResponseWithID { Message = "Header format not mentioned" };

                headerType = model.Header.Format;
                var headerFormat = (TemplateHeaderEnum)model.Header.Format;
                var headerMediaTypes = new List<TemplateHeaderEnum> { TemplateHeaderEnum.IMAGE, TemplateHeaderEnum.VIDEO, TemplateHeaderEnum.DOCUMENT };
                if (headerMediaTypes.Contains(headerFormat))
                {
                    //Media related validations
                    if (model.MediaId <= 0)
                        return new UResponseWithID { Message = "Media is required when header type is not text" };

                    var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID { Message = "Media not exist" };

                    if (mediaDetail.SenderNameId != model.SenderNameId)
                        return new UResponseWithID { Message = "Media does not exist for this sender" };

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType(headerFormat, mediaDetail.FileExtension);
                    if (!allowedMedia)
                        return new UResponseWithID { Message = $"Not allowed media for header type - {headerFormat}" };

                }

                if (headerFormat == TemplateHeaderEnum.TEXT)
                {
                    if (String.IsNullOrWhiteSpace(model.Header.Text))
                        return new UResponseWithID { Message = "Header text is required" };

                    model.Header.Text = model.Header.Text.Trim();
                    headerText = model.Header.Text;

                    MatchCollection matches = regex.Matches(model.Header.Text);
                    if (matches.Count > 0)
                    {
                        if (model.Header.DynamicValue == null || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(model.Header.DynamicValue.ParamValue))
                            return new UResponseWithID { Message = "Header default parameter is required" };

                        if (matches.Count > 1)
                            return new UResponseWithID { Message = "Only one header parameter is allowed" };

                        if (matches[0].Value != model.Header.DynamicValue.ParamName)
                            return new UResponseWithID { Message = "Headere parameter passed does not match with header text" };

                        headerTextCount = matches.Count;
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Header,
                            ParamName = model.Header.DynamicValue.ParamName,
                            ParamDefaultValue = model.Header.DynamicValue.ParamValue,
                            Sequence = 0
                        });
                    }
                }
            }

            //If body exist
            if (model.Body != null)
            {
                if (String.IsNullOrWhiteSpace(model.Body.Text))
                    return new UResponseWithID { Message = "Body text is required" };

                model.Body.Text = model.Body.Text.Trim();
                bodyText = model.Body.Text;
                MatchCollection matches = regex.Matches(model.Body.Text);
                if (matches.Count > 0)
                {
                    if (model.Body.DynamicValues == null || model.Body.DynamicValues.Count == 0)
                        return new UResponseWithID { Message = "Body text parameter is required" };

                    var duplicate = matches.Select(x => x.Value).GroupBy(item => item).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                    if (duplicate.Any())
                        return new UResponseWithID { Message = "Cannot have duplicate parameters in body." };

                    if (matches.Count != model.Body.DynamicValues.Count)
                        return new UResponseWithID { Message = "Body text parameters is not matching with body text count" };

                    var passedEmpty = model.Body.DynamicValues.Any(x => String.IsNullOrWhiteSpace(x.ParamName) || String.IsNullOrWhiteSpace(x.ParamValue));
                    if (passedEmpty)
                        return new UResponseWithID { Message = "Parameter name or value is not passed correctly in body" };

                    var matchValues = matches.Select(x => x.Value).ToList();
                    var bodyParams = model.Body.DynamicValues.Select(x => x.ParamName).ToList();

                    var areEqual = matchValues.All(item => bodyParams.Contains(item));
                    if (!areEqual)
                        return new UResponseWithID { Message = "Body parameters passed does not match with body text" };

                    bodyTextCount = matches.Count;
                    for (int i = 0; i < model.Body.DynamicValues.Count; i++)
                    {
                        var value = model.Body.DynamicValues[i];
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Body,
                            ParamName = value.ParamName,
                            ParamDefaultValue = value.ParamValue,
                            Sequence = i
                        });
                    }
                }
            }

            if (model.Footer != null)
                footerText = model.Footer.Text.Trim();

            if (model.Buttons != null && model.Buttons.Count > 0)
            {
                for (int i = 0; i < model.Buttons.Count; i++)
                {
                    var button = model.Buttons[i];
                    if (button.ActionType == (int)ActionTypeEnum.TEMPLATE && button.ActionId <= 0)
                        return new UResponseWithID { Message = $"Template id required in action id when action type is {(int)ActionTypeEnum.TEMPLATE}" };

                    button.ButtonValue = (button.ButtonValue ?? "").Trim();

                    if (!String.IsNullOrWhiteSpace(button.ButtonValue))
                    {
                        MatchCollection matches = regex.Matches(button.ButtonValue);
                        if (matches.Count > 0)
                        {
                            if (button.ButtonType != (int)ButtonTypeEnum.URL)
                                return new UResponseWithID { Message = $"Dynamic parameter not allowed in button type {(ButtonTypeEnum)button.ButtonType}" };

                            if (button.DynamicValue == null || String.IsNullOrWhiteSpace(button.DynamicValue.ParamName) || String.IsNullOrWhiteSpace(button.DynamicValue.ParamValue))
                                return new UResponseWithID { Message = "Button default parameter is required" };

                            if (matches.Count > 1)
                                return new UResponseWithID { Message = "Only one button parameter is allowed" };

                            if (matches[0].Value != button.DynamicValue.ParamName)
                                return new UResponseWithID { Message = $"Button - {button.ButtonText} parameter passed does not match with button value" };

                            parameters.Add(new TemplateDto.TemplateParameter
                            {
                                ParamType = (int)TemplateParamEnum.Button,
                                ParamName = button.DynamicValue.ParamName,
                                ParamDefaultValue = button.DynamicValue.ParamValue,
                                Sequence = button.Sequence //Assigning button sequence
                            });
                        }
                    }

                    buttons.Add(new TemplateDto.TemplateButton
                    {
                        ButtonType = button.ButtonType,
                        ButtonText = button.ButtonText,
                        ButtonValue = button.ButtonValue,
                        ActionId = button.ActionId,
                        ActionType = button.ActionType,
                        Sequence = button.Sequence,
                        SytemActionId = button.SytemActionId
                    });
                }
            }

            var parameterJson = JsonConvert.SerializeObject(parameters);
            var buttonJson = JsonConvert.SerializeObject(buttons);

            var responseList = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={clientId}, @TemplatesId={model.Id}, @SenderId={model.SenderNameId},  @TemplateName={model.Name},@Category={model.Category}, @Language={model.Language}, @HeaderType={headerType}, @HeaderParamCount={headerTextCount}, @HeaderText={headerText},@MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyTextCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={userId}").ToListAsync();

            await _cacheService.RemoveByPrefix(CacheKeys.TEMPLATE_PATTERN_KEY);

            if (responseList == null || !responseList.Any())
                return new UResponseWithID { Message = "Cannot add template" };

            var response = responseList[0];
            if (response.Status <= 0)
                return new UResponseWithID { Message = response.Message };

            //Push template to facebook
            return await PushTemplateToFacebook(clientId, response.Id);
        }

        private async Task<UResponseWithID> PushTemplateToFacebook(int clientId, int templateId)
        {
            var model = await GetTemplateDetailAsync(clientId, templateId);
            if (model == null)
                return new UResponseWithID { Message = "Cannot fetch template details" };

            var templateRequest = new TemplateRequestDto
            {
                ClientId = model.ClientId.ToString(),
                SenderNameId = model.SenderId.ToString(),
                Name = model.TemplateName,
                Category = model.Category,
                LanguageCode = model.Language,
                TemplateId = model.TemplateId,
            };

            string pattern = CommonHelper.DynamicPattern;

            #region Header
            string headerText = String.Empty;
            string headerMediaUrl = String.Empty;

            if (model.TemplateTypeId != 2)
            {
                // Process header text
                if (!String.IsNullOrWhiteSpace(model.HeaderText))
                {
                    int placeholderCount = 0;
                    headerText = Regex.Replace(model.HeaderText, pattern, match =>
                    {
                        placeholderCount++;
                        return $"{{{{{placeholderCount}}}}}";
                    });
                }

                // Process media
                if (model.MediaId > 0)
                {
                    var media = await _dbContext.Medias.FindAsync(model.MediaId);
                    headerMediaUrl = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
                }

                // Set header parameters
                var headerParam = model.Parameters
                    .Where(x => x.ParamType == (int)TemplateParamEnum.Header)
                    .Select(x => new { x.ParamName, x.ParamDefaultValue })
                    .FirstOrDefault();

                templateRequest.Header = new TemplateRequestDto.HeaderDto
                {
                    Format = ((TemplateHeaderEnum)model.HeaderType).ToString(),
                    MediaUrl = headerMediaUrl,
                    Text = headerText,
                    Example = headerParam != null ? headerParam.ParamDefaultValue : String.Empty
                };
            }
            #endregion

            #region Body

            if (!String.IsNullOrWhiteSpace(model.BodyText))
            {
                //Replace the {{example}} with {{1}} and so on...
                int placeholderCount = 0;
                string bodyText = Regex.Replace(model.BodyText, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });

                var bodyParams = model.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body && x.TemplateScreenId ==null).ToDictionary(item => item.ParamName, item => item.ParamDefaultValue);

                // Create a list to store default values in the order they appear
                var bodyDefaultValues = new List<string>();

                // Regex to match placeholders
                MatchCollection matches = Regex.Matches(model.BodyText, pattern);
                foreach (Match match in matches)
                {
                    string paramName = match.Value; // Get the matched placeholder
                    if (bodyParams.ContainsKey(paramName))
                        bodyDefaultValues.Add(bodyParams[paramName]); // Add the corresponding default value
                }

                templateRequest.Body = new TemplateRequestDto.BodyDto
                {
                    Text = bodyText,
                    Examples = bodyDefaultValues
                };
            }

            #endregion

            #region Cards 
            if (model.Screens != null)
            {
                var cardheader = new TemplateRequestDto.HeaderDto();
                var cardBody = new TemplateRequestDto.BodyDto();
                var cardFlow = new List<TemplateRequestDto.FlowComponent>();
                var cards = new List<TemplateRequestDto.CardDto>();
                foreach (var screen in model.Screens)
                {
                    //processing screen header
                    if (screen.MediaId > 0)
                    {
                        var Headermedia = await _dbContext.Medias.FindAsync(screen.MediaId);
                        headerMediaUrl = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, Headermedia.MediaPath);
                    }
                    cardheader = new TemplateRequestDto.HeaderDto
                    {
                        Format = ((TemplateHeaderEnum)screen.HeaderType).ToString(),
                        MediaUrl = headerMediaUrl
                    };

                    // processing the bodytext
                    if (!String.IsNullOrWhiteSpace(screen.BodyText))
                    {
                        //Replace the {{example}} with {{1}} and so on...
                        int placeholderCount = 0;
                        string CardbodyText = Regex.Replace(screen.BodyText, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });

                        var bodyParams = model.Parameters.Where(x => x.ParamType == (int)CarouselParamEnum.CardBody && x.TemplateScreenId == screen.TemplateScreenId).ToDictionary(item => item.ParamName, item => item.ParamDefaultValue);

                        // Create a list to store default values in the order they appear
                        var bodyDefaultValues = new List<string>();

                        // Regex to match placeholders
                        MatchCollection matches = Regex.Matches(screen.BodyText, pattern);
                        foreach (Match match in matches)
                        {
                            string paramName = match.Value; // Get the matched placeholder
                            if (bodyParams.ContainsKey(paramName))
                                bodyDefaultValues.Add(bodyParams[paramName]); // Add the corresponding default value
                        }

                        cardBody=new TemplateRequestDto.BodyDto
                        {
                            Text = CardbodyText,
                            Examples = bodyDefaultValues
                        };
                    }
                    if (screen != null && model.Buttons.Count > 0)
                    {
                        var cardButton = new List<TemplateRequestDto.ButtonDto>();
                        if (model.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW))
                        {
                            var button = model.Buttons.FirstOrDefault(x => x.ActionType == (int)ActionTypeEnum.FLOW);
                            var flow = await _dbContext.Flows.FindAsync(button.ActionId);
                            if (flow == null)
                                return new UResponseWithID { Status = 0, Message = $"Flow not found with id - {button.ActionId}" };

                            if (String.IsNullOrWhiteSpace(flow.MetaFlowId))
                                return new UResponseWithID { Status = 0, Message = $"Flow with id - {button.ActionId} does not have meta id yet" };

                            if ((flow.Status ?? "").ToLower() != FlowStatusEnum.PUBLISHED.ToString().ToLower())
                                return new UResponseWithID { Status = 0, Message = $"Flow is not published with id - {button.ActionId}" };

                            cardFlow.Add(new TemplateRequestDto.FlowComponent
                            {
                                FlowId = flow.MetaFlowId,
                                ButtonText = button.ButtonText
                            });
                        }
                        else
                        {
                            var cardbuttons = model.Buttons.FindAll(x => x.TemplateScreenId == screen.TemplateScreenId);
                            foreach (var item in cardbuttons)
                            {
                                var buttonType = ((ButtonTypeEnum)item.ButtonType);
                                string buttonValue = item.ButtonValue ?? "";
                                UTemplateDetail.Parameter buttonParam = null;
                                if (buttonType == ButtonTypeEnum.URL)
                                {
                                    //Replace the {{example}} with {{1}} and so on...
                                    int placeholderCount = 0;
                                    buttonValue = Regex.Replace(item.ButtonValue, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });

                                    MatchCollection matches = Regex.Matches(item.ButtonValue, pattern);
                                    if (matches != null && matches.Count > 0)
                                        buttonParam = model.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button && x.ParamName == matches[0].Value).FirstOrDefault();
                                }

                                //Replace country code seperation, +965-99310864 -> +96599310864
                                if (buttonType == ButtonTypeEnum.PHONE_NUMBER)
                                    buttonValue = buttonValue.Replace("-", "");

                                 cardButton.Add(new TemplateRequestDto.ButtonDto
                                {
                                    Type = buttonType.ToString(),
                                    Text = item.ButtonText,
                                    PhoneNumber = buttonType == ButtonTypeEnum.PHONE_NUMBER ? buttonValue : String.Empty,
                                    Url = buttonType == ButtonTypeEnum.URL ? buttonValue : String.Empty,
                                    Example = buttonParam != null ? buttonParam.ParamDefaultValue : String.Empty
                                });
                            }
                            cards.Add(new TemplateRequestDto.CardDto
                            {
                                Header = cardheader,
                                Body = cardBody,
                                Buttons = cardButton,
                            });
                        }

                    }

                }
                
                templateRequest.Cards = cards;
            }


            #endregion

            #region Footer
            if (model.TemplateTypeId != 2)
            {
                if (!String.IsNullOrWhiteSpace(model.FooterText))
                {
                    templateRequest.Footer = new TemplateRequestDto.FooterDto
                    {
                        Text = model.FooterText
                    };
                }
            }

            #endregion

            #region Button
            if (model.Buttons != null && model.Buttons.Count > 0 && model.TemplateTypeId == 1)
            {
                if (model.Buttons.Any(x => x.ActionType == (int)ActionTypeEnum.FLOW))
                {
                    var button = model.Buttons.FirstOrDefault(x => x.ActionType == (int)ActionTypeEnum.FLOW);
                    var flow = await _dbContext.Flows.FindAsync(button.ActionId);
                    if (flow == null)
                        return new UResponseWithID { Status = 0, Message = $"Flow not found with id - {button.ActionId}" };

                    if (String.IsNullOrWhiteSpace(flow.MetaFlowId))
                        return new UResponseWithID { Status = 0, Message = $"Flow with id - {button.ActionId} does not have meta id yet" };

                    if ((flow.Status ?? "").ToLower() != FlowStatusEnum.PUBLISHED.ToString().ToLower())
                        return new UResponseWithID { Status = 0, Message = $"Flow is not published with id - {button.ActionId}" };

                    templateRequest.Flow = new TemplateRequestDto.FlowComponent
                    {
                        FlowId = flow.MetaFlowId,
                        ButtonText = button.ButtonText
                    };
                }
                else
                {
                    foreach (var item in model.Buttons)
                    {
                        var buttonType = ((ButtonTypeEnum)item.ButtonType);
                        string buttonValue = item.ButtonValue ?? "";

                        UTemplateDetail.Parameter buttonParam = null;
                        if (buttonType == ButtonTypeEnum.URL)
                        {
                            //Replace the {{example}} with {{1}} and so on...
                            int placeholderCount = 0;
                            buttonValue = Regex.Replace(item.ButtonValue, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });

                            MatchCollection matches = Regex.Matches(item.ButtonValue, pattern);
                            if (matches != null && matches.Count > 0)
                                buttonParam = model.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button && x.ParamName == matches[0].Value).FirstOrDefault();
                        }
                        //Replace country code seperation, +965-99310864 -> +96599310864
                        if (buttonType == ButtonTypeEnum.PHONE_NUMBER)
                            buttonValue = buttonValue.Replace("-", "");

                        templateRequest.Buttons.Add(new TemplateRequestDto.ButtonDto
                        {
                            Type = buttonType.ToString(),
                            Text = item.ButtonText,
                            PhoneNumber = buttonType == ButtonTypeEnum.PHONE_NUMBER ? buttonValue : String.Empty,
                            Url = buttonType == ButtonTypeEnum.URL ? buttonValue : String.Empty,
                            Example = buttonParam != null ? buttonParam.ParamDefaultValue : String.Empty
                        });
                    }
                }
            }

            #endregion

            var request = Newtonsoft.Json.JsonConvert.SerializeObject(templateRequest);

            var apiCallStart = DateTime.UtcNow;
            string apiEndpoint = string.Empty;
            if (model.TemplateTypeId == (int)TemplateTypeEnum.Template)
            {
                apiEndpoint = $"/api/Template/TemplateMessageOps";
            }
            else
            {
                apiEndpoint = $"/api/Template/CarouselTemplateMessageOps";
            }

            var res = new StringContent(request, Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync(apiEndpoint, res);
            var content = await response1.Content.ReadAsStringAsync();

            _logger.LogInformation("Calling bridge API apiEndpoint={apiEndpoint} Template TemplateMessageOps with request={request} and response={response} with apiResponseTime={apiResponseTime}", apiEndpoint, request, content, DateTime.UtcNow.Subtract(apiCallStart).TotalMilliseconds);

            var result = JsonConvert.DeserializeObject<SyncResultDto>(content);
            if (result != null && result.success)
            {
                var data = JsonConvert.SerializeObject(result.result);
                var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateResultDto>(data);
                if (tempResult != null)
                {
                    if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                    {
                        var updateTemp = new TemplateStatusUpdateDto
                        {
                            Id = model.Id,
                            TemplateId = tempResult.id,
                            Status = tempResult.status,
                            Category = tempResult.category
                        };

                        var updateTemplate = await UpdateTemplateStatusByIdAsync(updateTemp);
                        if (updateTemplate == null || updateTemplate.Status <= 0)
                        {
                            return new UResponseWithID
                            {
                                Id = model.Id,
                                Status = 201, //Created but not created in facebook
                                Message = "Template created in system but not on facebook because: \n" + updateTemplate?.Message
                            };
                        }

                        return new UResponseWithID
                        {
                            Status = 200,
                            Message = "Template added successfully"
                        };
                    }
                }
            }
            else if (result != null && !result.success)
            {
                return new UResponseWithID
                {
                    Status = 201, //Created but not created in facebook
                    Message = "Template created in system but not created on facebook because: \n" + result.message,
                    Id = model.Id
                };
            }


            return new UResponseWithID { Message = "Something went wrong while sending request to facebook" };
        }

        public async Task<UResponseWithID> DeleteTemplateAsync(int Id)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Delete}, @TemplatesId={Id}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.TEMPLATE_PATTERN_KEY);
            return response[0];
        }

        public async Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateStatusUpdateDto model)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.UpdateTemplateStatus}, @TemplatesId={model.Id}, @TemplateId={model.TemplateId}, @Status={model.Status}, @Category={model.Category}, @ActionBy={model.ActionBy}").ToListAsync();
            await _cacheService.RemoveByPrefix(CacheKeys.TEMPLATE_PATTERN_KEY);
            return response[0];
        }

        public async Task<UTemplateDetail> GetTemplateDetailAsync(int clientId, int templateId)
        {
            var cacheKey = string.Format(CacheKeys.TEMPLATE_BY_ID_KEY, clientId, templateId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.TemplateDetails.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateDetails}, @ClientId={clientId}, @TemplatesId={templateId}").ToListAsync();

                _logger.LogInformation("Calling procedure usp_Templates_Ops with parameters: ActionId={ActionId}, ClientId={ClientId}, TemplatesId={TemplatesId}, " +
                    "ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.GetTemplateDetails, clientId, templateId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

                if (response != null && response.Any())
                {
                    var template = response[0];
                    template.Buttons = !String.IsNullOrWhiteSpace(template.ButtonsJson)
                        ? JsonConvert.DeserializeObject<List<UTemplateDetail.Button>>(template.ButtonsJson)
                        : new List<UTemplateDetail.Button>();

                    template.Parameters = !String.IsNullOrWhiteSpace(template.ParametersJson)
                        ? JsonConvert.DeserializeObject<List<UTemplateDetail.Parameter>>(template.ParametersJson)
                        : new List<UTemplateDetail.Parameter>();

                    template.Screens = !String.IsNullOrWhiteSpace(template.ScreensJson)
                     ? JsonConvert.DeserializeObject<List<UTemplateDetail.Screen>>(template.ScreensJson)
                     : new List<UTemplateDetail.Screen>();

                    return template;
                }

                return null;
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);

            return cacheResult;
        }

        public async Task<List<UEntityDto>> GetTemplatesAsync(int clientId, int senderId = 0, string searchStr = "", string cat = "")
        {
            var cacheKey = string.Format(CacheKeys.TEMPLATE_DROPDOWN_KEY, clientId, senderId, searchStr, cat);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_carousel_ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId}, @SenderId={senderId}, @SearchStr={searchStr}, @Category={cat}").ToListAsync();
                _logger.LogInformation("Calling procedure usp_Templates_Ops with parameters: " +
                    "ActionId={ActionId}, ClientId={ClientId}, SenderId={SenderId}, SearchStr={SearchStr}, " +
                    "ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.GetEntities, clientId, senderId, searchStr, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                return response;
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);

            return cacheResult;

        }

        public async Task<List<UEntity2Dto>> GetTemplateCategoriesAsync(string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.TEMPLATE_DROPDOWN_KEY2, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.Entity2.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateCategories}, @SearchStr={searchStr}").ToListAsync();
                _logger.LogInformation("Calling procedure usp_Templates_Ops with searchStr={searchStr}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", searchStr, (int)CrudEnum.GetTemplateCategories, CrudEnum.GetTemplateCategories, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                return response;
            });
            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;

        }

        public async Task<List<UEntity2Dto>> GetLanguagesAsync(string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.TEMPLATE_DROPDOWN_KEY3, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var startProcTime = DateTime.UtcNow;
                var response = await _dbContext2.Entity2.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetLanguages}, @SearchStr={searchStr}").ToListAsync();
                _logger.LogInformation("Calling procedure usp_Templates_Ops with searchStr={searchStr}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime}", searchStr, (int)CrudEnum.GetLanguages, CrudEnum.GetLanguages, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
                return response;
            });

            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);

            return cacheResult;
        }

        public async Task<UResponseWithID> AddCarouselTemplateAsync(int clientId, int useId, TemplateDto requestDto)
        {
            requestDto.Name = requestDto.Name.Replace(" ", "_").ToLower().Trim();

            // Check if already exists
            var templateNameExist = await _dbContext.Templates
                .Where(x => x.ClientId == clientId &&
                            x.SenderId == requestDto.SenderNameId &&
                            x.TemplateName != null &&
                            x.TemplateName.ToLower() == requestDto.Name.ToLower() && x.TemplateTypeId == (int)TemplateTypeEnum.Carousel)
                .FirstOrDefaultAsync();

            if (templateNameExist != null)
                return new UResponseWithID { Message = "Carousel template with same name already exists" };

            Regex regex = new Regex(CommonHelper.DynamicPattern);
            List<CreateCarouselTemplateRequestDto.CaraouselParameter> parameters = new();
            List<CreateCarouselTemplateRequestDto.CaraouselButton> buttons = new();
            List<CreateCarouselTemplateRequestDto.CarouselScreen> screens = new();
            int headerType = 0;
            string mainBodyText = String.Empty;
            int BodyTextCount = 0;
            string bodyText = String.Empty;
            int bodyTextCount = 0;
            int templateParamSequence = 0;
            var allowedcardheaderformate = requestDto.Cards[0].Header.Format;
            int allowedbuttoncount = requestDto.Cards[0].Buttons.Count;
            if (requestDto.Body != null)
            {

                // Prepare main body text for the carousel template
                if (string.IsNullOrWhiteSpace(requestDto.Body.Text))
                    return new UResponseWithID { Message = "Body text is required" };

                requestDto.Body.Text = requestDto.Body.Text.Trim();
                //Assigning the main  body text to store in db;
                mainBodyText = requestDto.Body.Text;

                MatchCollection matches = regex.Matches(requestDto.Body.Text);
                if (matches.Count > 0)
                {
                    if (requestDto.Body.DynamicValues == null || !requestDto.Body.DynamicValues.Any())
                        return new UResponseWithID { Message = "Body dynamic parameters are required" };

                    if (matches.Count > 10)
                        return new UResponseWithID { Message = "Only 10 dynamic parameters are allowed in Body" };

                    var matchValues = matches.Select(x => x.Value).ToList();
                    var bodyParams = requestDto.Body.DynamicValues.Select(x => x.ParamName).ToList();

                    var areEqual = matchValues.All(item => bodyParams.Contains(item));
                    if (!areEqual)
                        return new UResponseWithID { Message = "Body parameters passed does not match with body text" };

                    BodyTextCount = matches.Count;

                    foreach (var kv in requestDto.Body.DynamicValues)
                    {
                        parameters.Add(new CreateCarouselTemplateRequestDto.CaraouselParameter
                        {
                            ParamType = (int)CarouselParamEnum.TemplateBody,
                            ParamName = kv.ParamName,
                            ParamDefaultValue = kv.ParamValue,
                            Sequence = 0
                        });
                    }
                }
            }
            if (requestDto.Cards.Any(card => card.Header.Format != allowedcardheaderformate))
            {

                return new UResponseWithID
                {
                    Message = "All the headers Formate in the card should match"
                };
            }
            if (requestDto.Cards.Any(card => card.Buttons.Count != allowedbuttoncount))
            {
                return new UResponseWithID
                {
                    Message = "All the cards should have the same number of Buttons"
                };
            }

            // preparing the card :- CardHeader, CardBody, CardButtons
            for (int cardIndex = 0; cardIndex < requestDto.Cards.Count; cardIndex++)
            {
                int BodyParamCount = 0;
                string BodyText = string.Empty;
                var card = requestDto.Cards[cardIndex];


                // --- Card Header ---
                if (card.Header != null)
                {
                    // if header format is none then
                    if (card.Header.Format < 0)
                        return new UResponseWithID { Message = $"Card {cardIndex + 1}: Header format not mentioned" };
                    headerType = card.Header.Format;
                    var headerFormat = (TemplateHeaderEnum)card.Header.Format;
                    var CardheaderMediaTypes = new List<TemplateHeaderEnum> { TemplateHeaderEnum.IMAGE, TemplateHeaderEnum.VIDEO };

                    // if it contains image, audio
                    if (CardheaderMediaTypes.Contains(headerFormat))
                    {
                        // MediaId should now come from the card.Header
                        if (card.Header.MediaId <= 0)
                            return new UResponseWithID { Message = $"Card {cardIndex + 1}: Media is required for media header" };

                        var mediaDetail = await _dbContext.Medias.FindAsync(card.Header.MediaId);
                        if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                            return new UResponseWithID { Message = $"Card {cardIndex + 1}: Media not found" };

                        if (mediaDetail.SenderNameId != requestDto.SenderNameId)
                            return new UResponseWithID { Message = $"Card {cardIndex + 1}: Media does not belong to this sender" };

                        var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType(headerFormat, mediaDetail.FileExtension);
                        if (!allowedMedia)
                            return new UResponseWithID { Message = $"Card {cardIndex + 1}: Not allowed media type for header" };
                    }
                    else
                    {
                        return new UResponseWithID { Message = "Only Image and Video is allowed in header" };
                    }

                }
                else
                {
                    return new UResponseWithID { Message = "Card Header is required" };

                }
                // Handling Body
                if (card.Body != null)
                {
                    if (string.IsNullOrWhiteSpace(card.Body.Text))
                        return new UResponseWithID { Message = "Body text is required" };

                    card.Body.Text = card.Body.Text.Trim();
                    BodyText = card.Body.Text;

                    MatchCollection matches = regex.Matches(BodyText);
                    BodyParamCount = matches.Count;
                    if (matches.Count > 0)
                    {
                        if (card.Body.DynamicValues == null || !card.Body.DynamicValues.Any())
                            return new UResponseWithID { Message = "Body dynamic parameters are required" };

                        if (matches.Count > 2)
                            return new UResponseWithID { Message = "Only 2 dynamic parameters are allowed in card body" };

                        var paramNames = card.Body.DynamicValues.Select(d => d.ParamName).ToHashSet();
                        var paramValues = card.Body.DynamicValues.Select(d => d.ParamValue).ToHashSet();
                        foreach (Match match in matches)
                        {
                            if (!paramNames.Contains(match.Value))
                                return new UResponseWithID { Message = $"Card's Body parameter '{match.Value}' is missing in dynamic values" };
                        }

                        foreach (var kv in card.Body.DynamicValues)
                        {
                            parameters.Add(new CreateCarouselTemplateRequestDto.CaraouselParameter
                            {
                                ParamType = (int)CarouselParamEnum.CardBody,
                                ParamName = kv.ParamName,
                                ParamDefaultValue = kv.ParamValue,
                                Sequence = cardIndex,
                                ScreenSequence = cardIndex + 1
                            });
                        }
                    }

                }
                // --- Buttons ---
                if (card.Buttons != null && card.Buttons.Count > 0)
                {
                    foreach (var button in card.Buttons)
                    {
                        if (button.ActionType == (int)ActionTypeEnum.Carousel && button.ActionId <= 0)
                            return new UResponseWithID { Message = $"Card {cardIndex + 1}: Template ID required for button action" };

                        button.ButtonValue = (button.ButtonValue ?? "").Trim();

                        MatchCollection matches = regex.Matches(button.ButtonValue);
                        if (matches.Count > 0)
                        {
                            if (button.ButtonType != (int)ButtonTypeEnum.URL)
                                return new UResponseWithID { Message = $"Card {cardIndex + 1}: Dynamic value only allowed in URL button" };

                            if (matches.Count > 1)
                                return new UResponseWithID { Message = $"Card {cardIndex + 1}: Only one dynamic value allowed in button" };

                            if (button.DynamicValue == null || string.IsNullOrWhiteSpace(button.DynamicValue.ParamName) || string.IsNullOrWhiteSpace(button.DynamicValue.ParamValue))
                                return new UResponseWithID { Message = $"Card {cardIndex + 1}: Dynamic value required in button" };

                            if (button.DynamicValue.ParamName != button.DynamicValue.ParamName)
                                return new UResponseWithID { Message = $"Card {cardIndex + 1}: Button parameter mismatch" };

                            parameters.Add(new CreateCarouselTemplateRequestDto.CaraouselParameter
                            {
                                ParamType = (int)TemplateParamEnum.Button,
                                ParamName = button.DynamicValue.ParamName,
                                ParamDefaultValue = button.DynamicValue.ParamValue,
                                Sequence = templateParamSequence,
                                ScreenSequence = cardIndex + 1
                            });
                        }

                        buttons.Add(new CreateCarouselTemplateRequestDto.CaraouselButton
                        {
                            ButtonType = button.ButtonType,
                            ButtonText = button.ButtonText,
                            ButtonValue = button.ButtonValue,
                            ActionId = button.ActionId,
                            ActionType = button.ActionType,
                            Sequence = cardIndex,
                            SytemActionId = button.SytemActionId,
                            ScreenSequence = cardIndex+1
                        });
                    }
                    screens.Add(new CreateCarouselTemplateRequestDto.CarouselScreen
                    {
                        Sequence = cardIndex + 1,
                        HeaderType = headerType,
                        MediaId = card.Header?.MediaId ?? 0,
                        HeaderParamCount = 0,
                        HeaderText = "",
                        BodyParamCount = BodyParamCount,
                        BodyText = BodyText
                    });
                }
            }

            var parameterJson = JsonConvert.SerializeObject(parameters);
            var buttonJson = JsonConvert.SerializeObject(buttons);
            var screenJson = JsonConvert.SerializeObject(screens);
            var responseList = await _dbContext2.ResponseWithID.FromSqlInterpolated($"EXEC usp_carousel_ops @ActionId = {(int)CrudEnum.Add},@ClientId = {clientId},@SenderId = {requestDto.SenderNameId},@TemplateName = {requestDto.Name},@Category = {requestDto.Category},@Language = {requestDto.Language},@BodyText = {requestDto.Body?.Text},@ButtonsJson = {buttonJson},@ParametersJson = {parameterJson},@ScreensJson = {screenJson},@ActionBy = {useId}").ToListAsync();

            if (responseList == null || !responseList.Any())
                return new UResponseWithID { Message = "Cannot add carousel template" };

            var response = responseList[0];
            if (response.Status <= 0)
                return new UResponseWithID
                {
                    Message = response.Message
                };

            return await PushTemplateToFacebook(clientId, response.Id);

        }
    }
}
