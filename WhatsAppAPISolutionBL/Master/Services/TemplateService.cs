using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Template;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.Setting;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Template;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;
        private readonly IOptions<APISolutionConfigurationSettings> _apiSolutionConfigurationSettings;
        private readonly IMediaService _mediaService;

        public TemplateService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IHttpClientFactory httpClientFactory,
            IOptions<APISolutionConfigurationSettings> apiSolutionConfigurationSettings,
            IMediaService mediaService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            _apiSolutionConfigurationSettings = apiSolutionConfigurationSettings;
            _mediaService = mediaService;
        }

        public async Task<List<UTemplate>> GetTemplateListAsync(int clientId, int transactionType = 0, string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.Templates.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @TransactionType={transactionType}, @SearchStr={searchStr},@SortBy={sortBy},@PageNo={pageNo},@PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponseWithID> AddTemplateAsync(TemplateDto model)
        {
            //Replace empty space with _
            model.Name = model.Name.Replace(" ", "_").ToLower().Trim();

            //Check if template name already exists
            var templateNameExist = await _dbContext.Templates
                .Where(x => x.ClientId == model.ClientId
                && x.SenderId == model.SenderNameId
                && x.RecordStatus != -1
                && x.TemplateName != null
                && x.Language != null
                && x.TemplateName.ToLower() == model.Name.ToLower()
                && x.Language.ToLower() == model.Language.ToLower()).FirstOrDefaultAsync();

            if (templateNameExist != null)
                return new UResponseWithID { Status = 0, Message = "Template with same name already exist" };

            //Globals
            //Regex regex = new Regex(@"{{\d+}}");
            Regex regex = new Regex(@"\{\{.*?\}\}");
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
                if (model.Header.Format <= 0)
                    return new UResponseWithID { Status = 0, Message = "Header format not mentioned" };

                headerType = model.Header.Format;
                var headerFormat = (TemplateHeaderEnum)model.Header.Format;
                var headerMediaTypes = new List<TemplateHeaderEnum> { TemplateHeaderEnum.IMAGE, TemplateHeaderEnum.VIDEO, TemplateHeaderEnum.DOCUMENT };
                if (headerMediaTypes.Contains(headerFormat))
                {
                    //Media related validations
                    if (model.MediaId <= 0)
                        return new UResponseWithID { Status = 0, Message = "Media is required when header type is not text" };

                    var mediaDetail = await _dbContext.Medias.FindAsync(model.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID { Status = 0, Message = "Media not exist" };

                    if (mediaDetail.SenderNameId != model.SenderNameId)
                        return new UResponseWithID { Status = 0, Message = "Media does not exist for this sender" };

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType(headerFormat, mediaDetail.FileExtension);
                    if (!allowedMedia)
                        return new UResponseWithID { Status = 0, Message = $"Not allowed media for header type - {headerFormat}" };

                }

                if (headerFormat == TemplateHeaderEnum.TEXT)
                {
                    if (String.IsNullOrWhiteSpace(model.Header.Text))
                        return new UResponseWithID { Status = 0, Message = "Header text is required" };

                    model.Header.Text = model.Header.Text.Trim();
                    headerText = model.Header.Text;

                    MatchCollection matches = regex.Matches(model.Header.Text);
                    if (matches.Count > 0)
                    {
                        if (model.Header.DynamicValue == null)
                            return new UResponseWithID { Status = 0, Message = "Header default parameter is required" };

                        if (matches.Count > 1)
                            return new UResponseWithID { Status = 0, Message = "Only one header parameter is allowed" };

                        headerTextCount = matches.Count;
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Header,
                            ParamName = model.Header.DynamicValue.ParamName,
                            ParamDefaultValue = model.Header.DynamicValue.ParamValue,
                            Sequence = 1
                        });
                    }
                }
            }

            //If body exist
            if (model.Body != null)
            {
                if (String.IsNullOrWhiteSpace(model.Body.Text))
                    return new UResponseWithID { Status = 0, Message = "Body text is required" };

                model.Body.Text = model.Body.Text.Trim();
                bodyText = model.Body.Text;
                MatchCollection matches = regex.Matches(model.Body.Text);
                if (matches.Count > 0)
                {
                    var distinctMatch = matches.Select(x => x.Value).Distinct().ToList();

                    if (model.Body.DynamicValues == null || model.Body.DynamicValues.Count == 0)
                        return new UResponseWithID { Status = 0, Message = "Body text parameter is required" };

                    if (distinctMatch.Count() != model.Body.DynamicValues.Count)
                        return new UResponseWithID { Status = 0, Message = "Body text parameters is not matching with body text count" };

                    bodyTextCount = distinctMatch.Count();
                    for (int i = 0; i < model.Body.DynamicValues.Count; i++)
                    {
                        var value = model.Body.DynamicValues[i];
                        parameters.Add(new TemplateDto.TemplateParameter
                        {
                            ParamType = (int)TemplateParamEnum.Body,
                            ParamName = value.ParamName,
                            ParamDefaultValue = value.ParamValue,
                            Sequence = i + 1
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
                        return new UResponseWithID { Status = 0, Message = $"Template id required in action id when action type is {(int)ActionTypeEnum.TEMPLATE}" };

                    button.ButtonValue = (button.ButtonValue ?? "").Trim();

                    if (!String.IsNullOrWhiteSpace(button.ButtonValue))
                    {
                        MatchCollection matches = regex.Matches(button.ButtonValue);
                        if (matches.Count > 0)
                        {
                            if (button.ButtonType != (int)ButtonTypeEnum.URL)
                                return new UResponseWithID { Status = 0, Message = $"Dynamic parameter not allowed in button type {(ButtonTypeEnum)button.ButtonType}" };

                            if (button.DynamicValue == null)
                                return new UResponseWithID { Status = 0, Message = "Button default parameter is required" };

                            if (matches.Count > 1)
                                return new UResponseWithID { Status = 0, Message = "Only one button parameter is allowed" };

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

            var responseList = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={model.ClientId}, @SenderId={model.SenderNameId},  @TemplateName={model.Name},@Category={model.Category}, @Language={model.Language}, @HeaderType={headerType}, @HeaderParamCount={headerTextCount}, @HeaderText={headerText},@MediaId={model.MediaId}, @BodyText={bodyText}, @BodyParamCount={bodyTextCount}, @FooterText={footerText}, @ButtonsJson={buttonJson},@ParametersJson={parameterJson}, @ActionBy={model.ActionBy}").ToListAsync();
            if (responseList == null || !responseList.Any())
                return new UResponseWithID { Status = 0, Message = "Cannot add template" };

            var response = responseList[0];
            if (response.Status <= 0)
                return new UResponseWithID { Status = 0, Message = response.Message };

            //Push template to facebook
            return await PushTemplateToFacebook(model.ClientId, response.Id);
        }

        private async Task<UResponseWithID> PushTemplateToFacebook(int clientId, int templateId)
        {
            var model = await GetTemplateDetailAsync(clientId, templateId);
            if (model == null)
                return new UResponseWithID { Status = 0, Message = "Cannot fetch template details" };

            var tempateResponse = new TemplateRequestDto
            {
                ClientId = model.ClientId.ToString(),
                SenderNameId = model.SenderId.ToString(),
                Name = model.TemplateName,
                Category = model.Category,
                LanguageCode = model.Language,
            };

            string pattern = @"\{\{.*?\}\}";

            #region Header

            string headerText = String.Empty;
            string headerMediaUrl = String.Empty;
            if (!String.IsNullOrWhiteSpace(model.HeaderText))
            {
                int placeholderCount = 0;
                headerText = Regex.Replace(model.HeaderText, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });
            }

            if (model.MediaId > 0)
            {
                var media = await _dbContext.Medias.FindAsync(model.MediaId);
                headerMediaUrl = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, media.MediaPath);
            }

            var headerParam = model.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).Select(x => new { x.ParamName, x.ParamDefaultValue }).FirstOrDefault();
            tempateResponse.Header = new TemplateRequestDto.HeaderDto
            {
                Format = ((TemplateHeaderEnum)model.HeaderType).ToString(),
                MediaUrl = headerMediaUrl,
                Text = headerText,
                Example = headerParam != null ? headerParam.ParamDefaultValue : String.Empty
            };

            #endregion

            #region Body

            if (!String.IsNullOrWhiteSpace(model.BodyText))
            {
                //Replace the {{example}} with {{1}} and so on...
                int placeholderCount = 0;
                string bodyText = Regex.Replace(model.BodyText, pattern, match => { placeholderCount++; return $"{{{{{placeholderCount}}}}}"; });

                var bodyParams = model.Parameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToDictionary(item => item.ParamName, item => item.ParamDefaultValue);

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

                tempateResponse.Body = new TemplateRequestDto.BodyDto
                {
                    Text = bodyText,
                    Examples = bodyDefaultValues
                };
            }

            #endregion

            #region Footer

            if (!String.IsNullOrWhiteSpace(model.FooterText))
            {
                tempateResponse.Footer = new TemplateRequestDto.FooterDto
                {
                    Text = model.FooterText
                };
            }

            #endregion

            #region Button

            foreach (var item in model.Buttons)
            {
                var buttonType = ((ButtonTypeEnum)item.ButtonType);
                string buttonValue = item.ButtonValue;

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

                tempateResponse.Buttons.Add(new TemplateRequestDto.ButtonDto
                {
                    Type = buttonType.ToString(),
                    Text = item.ButtonText,
                    PhoneNumber = buttonType == ButtonTypeEnum.PHONE_NUMBER ? buttonValue : String.Empty,
                    Url = buttonType == ButtonTypeEnum.URL ? buttonValue : String.Empty,
                    Example = buttonParam != null ? buttonParam.ParamDefaultValue : String.Empty
                });
            }

            #endregion

            var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempateResponse), Encoding.UTF8, "application/json");
            var response1 = await _httpClient.PostAsync($"/api/Template/TemplateMessageOps", res);
            var content = await response1.Content.ReadAsStringAsync();

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

            return new UResponseWithID { Status = 0, Message = "Something went wrong while sending request to facebook" };
        }

        public async Task<UResponseWithID> DeleteTemplateAsync(int Id)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Delete}, @TemplatesId={Id}").ToListAsync();
            return response[0];
        }

        public async Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateStatusUpdateDto model)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.UpdateTemplateStatus}, @TemplatesId={model.Id}, @TemplateId={model.TemplateId}, @Status={model.Status}, @Category={model.Category}, @ActionBy={model.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UTemplateDetail> GetTemplateDetailAsync(int clientId, int templateId)
        {
            var response = await _dbContext2.TemplateDetails.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateDetails}, @ClientId={clientId}, @TemplatesId={templateId}").ToListAsync();
            if (response != null && response.Any())
            {
                var template = response[0];
                template.Buttons = !String.IsNullOrWhiteSpace(template.ButtonsJson)
                    ? JsonConvert.DeserializeObject<List<UTemplateDetail.Button>>(template.ButtonsJson)
                    : new List<UTemplateDetail.Button>();

                template.Parameters = !String.IsNullOrWhiteSpace(template.ParametersJson)
                    ? JsonConvert.DeserializeObject<List<UTemplateDetail.Parameter>>(template.ParametersJson)
                    : new List<UTemplateDetail.Parameter>();

                return template;
            }

            return null;
        }

        private async Task<List<UTemplateParameter>> GetTemplateParametersAsync(int client_Id, int template_Id = 0)
        {
            var response = await _dbContext2.TemplateParameters.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateParameterDetails}, @ClientId={client_Id}, @TemplatesId={template_Id}").ToListAsync();
            return response;
        }

        public async Task<List<UEntityDto>> GetTemplatesAsync(int clientId, int defaultType = 0, int senderId = 0, int transactionType = 0, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId}, @DefaultType={defaultType}, @SenderId={senderId},@TransactionType={transactionType}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UEntity2Dto>> GetTemplateCategoriesAsync(string searchStr = "")
        {
            var response = await _dbContext2.Entity2.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateCategories}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UEntity2Dto>> GetLanguagesAsync(string searchStr = "")
        {
            var response = await _dbContext2.Entity2.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetLanguages}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<List<UDefaultTemplateList>> GetDefaultTemplateListAsync(int clientId, int senderId = 0, int templateId = 0, int defaultType = 0, string searchStr = "")
        {
            var response = await _dbContext2.GetDefaultTemplates.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetDefaultTemplates}, @ClientId={clientId}, @SenderId={senderId},@TemplatesId={templateId},  @DefaultType={defaultType}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }
    }
}
