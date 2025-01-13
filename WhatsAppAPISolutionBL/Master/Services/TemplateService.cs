using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text;
using System.Text.Json;
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
using static WhatsAppAPISolutionDL.Dto.Message.WhatsAppMessageReceiveDto;

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

        public async Task<UResponseWithID> AddTemplateAsync(TemplateDto template)
        {
            int headerType = 0;
            var headerText = "";
            int headerParamCount = 0;
            var bodyText = "";
            int bodyParamCount = 0;
            var footer = "";
            var mediaUrl = "";
            var headerValues = new List<TemplateDto.KeyValue>();
            var bodyValues = new List<TemplateDto.KeyValue>();
            var buttonValues = new List<TemplateParameter>();
            Regex regex = new Regex(@"{{\d+}}");

            template.Name = template.Name.Replace(" ", "_").ToLower().Trim();

            //Check if template name already exists
            var templateNameExist = await _dbContext.Templates
                .Where(x => x.RecordStatus == 1
                && x.ClientId == template.ClientId
                && x.SenderId == template.SenderNameId
                && x.TemplateName != null
                && x.Language != null
                && x.TemplateName.ToLower() == template.Name.ToLower()
                && x.Language.ToLower() == template.Language.ToLower()).FirstOrDefaultAsync();

            if (templateNameExist != null)
            {
                return new UResponseWithID
                {
                    Status = 0,
                    Message = "Template with same name already exist"
                };
            }

            if (template.Header != null)
            {
                if (template.Header.Format == (int)TemplateHeaderEnum.IMAGE
                    || template.Header.Format == (int)TemplateHeaderEnum.VIDEO
                    || template.Header.Format == (int)TemplateHeaderEnum.DOCUMENT)
                {
                    if (template.MediaId <= 0)
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Media is required when header type is not text"
                        };

                    var mediaDetail = await _dbContext.Medias.FindAsync(template.MediaId);
                    if (mediaDetail == null || string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };
                    else
                        mediaUrl = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, mediaDetail.MediaPath);

                    if (mediaDetail.SenderNameId != template.SenderNameId)
                    {
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Media does not exist for this sender"
                        };
                    }

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.Header.Format, mediaDetail.FileExtension);
                    if (!allowedMedia)
                    {
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.Header.Format}"
                        };
                    }
                }

                if (template.Header.Format == (int)TemplateHeaderEnum.TEXT && String.IsNullOrWhiteSpace(template.Header.Text))
                {
                    return new UResponseWithID
                    {
                        Status = 0,
                        Message = "Header text is required"
                    };
                }

                if (!string.IsNullOrEmpty(template.Header.Text))
                {
                    MatchCollection matches = regex.Matches(template.Header.Text);
                    if (matches.Count() > 1)
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Only one header parameters is allowed"
                        };
                    if (!(template.Header.TextCount == matches.Count))
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Header text parameters is not matching with header text count"
                        };
                }
                if (template.Header.TextCount == 0)
                    template.Header.Values = null;

                headerType = template.Header.Format;
                headerText = template.Header.Text;
                headerParamCount = template.Header.TextCount;
            }

            if (template.Body != null)
            {
                MatchCollection matches = regex.Matches(template.Body.Text);
                if (!(template.Body.TextCount == matches.Count))
                    return new UResponseWithID
                    {
                        Status = 0,
                        Message = "Body text parameters is not matching with body text count"
                    };

                if (template.Body.TextCount == 0)
                    template.Body.Values = null;

                bodyText = template.Body.Text;
                bodyParamCount = template.Body.TextCount;
            }

            if (template.Footer != null)
                footer = template.Footer.Text;

            if (template.Header != null && template.Header.Values != null && template.Header.Values.Any())
            {
                foreach (var val in template.Header.Values)
                {
                    var param = new TemplateDto.KeyValue
                    {
                        Index = val.Index,
                        Value = val.Value,
                        DefaultValue = val.DefaultValue
                    };
                    headerValues.Add(param);
                }
            }

            if (template.Body != null && template.Body.Values != null && template.Body.Values.Any())
            {
                foreach (var val in template.Body.Values)
                {
                    var param = new TemplateDto.KeyValue
                    {
                        Index = val.Index,
                        Value = val.Value,
                        DefaultValue = val.DefaultValue
                    };
                    bodyValues.Add(param);
                }
            }

            if (template.Buttons != null && template.Buttons.Any())
            {
                foreach (var button in template.Buttons)
                {
                    //if (button.TextCount == 0)
                    //    button.Values = null;

                    if (button.ActionType == (int)ActionTypeEnum.TEMPLATE && button.ActionId <= 0)
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Template Id required in action id when action type is template"
                        };

                    var param = new TemplateParameter
                    {
                        Sequence = button.Index,
                        ParamName = button.Text,
                        ParamText = String.Empty,
                        ParamDefaultValue = String.Empty, //button.Url,
                        IsDynamic = false,
                        ParamType = (int)TemplateParamEnum.Button,
                        ButtonType = button.Type,
                        ActionId = button.ActionId,
                        ActionType = button.ActionType,
                        ButtonId = button.ButtonId
                    };

                    // Handle URL button cases
                    if (button.Type == (int)ButtonTypeEnum.URL)
                    {
                        //if (button.TextCount == 0)
                        //{
                        //param.ParamText = button.Url; // Use URL as text
                        //}
                        //else if (button.TextCount == 1)
                        //{
                        param.ParamText = button.Url; // Use URL as text
                        param.ParamDefaultValue = button.Values != null && !String.IsNullOrWhiteSpace(button.Values.Value) ? button.Values.Value : "";
                        param.IsDynamic = button.Values != null && !String.IsNullOrWhiteSpace(button.Values.Value); // Mark as dynamic
                        //}
                    }
                    else if (button.Type == (int)ButtonTypeEnum.PHONE_NUMBER)
                    {
                        param.ParamText = button.PhoneNumber;
                        //param.ParamDefaultValue = button.PhoneNumber;
                    }

                    buttonValues.Add(param);
                }
            }

            var headerJson = JsonSerializer.Serialize(headerValues);
            var bodyJson = JsonSerializer.Serialize(bodyValues);
            var buttonJson = JsonSerializer.Serialize(buttonValues);

            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={template.ClientId}, @TemplateName={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderType={headerType}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @HeaderValues={headerJson}, @BodyValues={bodyJson}, @FooterText={footer}, @ButtonValues={buttonJson}, @TransactionType={template.TransactionType}, @MediaId={template.MediaId}, @SenderId={template.SenderNameId}, @ActionBy={template.ActionBy}, @DefaultType={template.DefaultType}").ToListAsync();
            if (response == null || !response.Any())
                return new UResponseWithID
                {
                    Status = 0,
                    Message = "Oops somethng went wrong"
                };

            if (response[0].Status > 0)
            {
                var tempateResponse = new TemplateRequestDto
                {
                    ClientId = template.ClientId.ToString(),
                    SenderNameId = template.SenderNameId.ToString(),
                    Name = template.Name,
                    Category = template.Category,
                    LanguageCode = template.Language,
                };

                tempateResponse.Header = new TemplateRequestDto.HeaderDto
                {
                    Format = ((TemplateHeaderEnum)template.Header.Format).ToString(),
                    MediaUrl = mediaUrl,
                    Text = template.Header.Text,
                    Example = template.Header.Values?.FirstOrDefault()?.Value ?? ""
                };

                tempateResponse.Body = new TemplateRequestDto.BodyDto
                {
                    Text = template.Body.Text,
                    Examples = template.Body?.Values?.Select(x => x.Value).ToList() ?? new List<string>()
                };

                tempateResponse.Footer = new TemplateRequestDto.FooterDto
                {
                    Text = template.Footer.Text
                };

                foreach (var item in template.Buttons)
                {
                    tempateResponse.Buttons.Add(new TemplateRequestDto.ButtonDto
                    {
                        Type = ((ButtonTypeEnum)item.Type).ToString(),
                        Text = item.Text,
                        PhoneNumber = item.PhoneNumber,
                        Url = item.Url,
                        Example = item.Values != null && !String.IsNullOrWhiteSpace(item.Values.Value) ? item.Values.Value : "",
                    });
                }

                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempateResponse), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/TemplateMessageOps", res);
                var content = await response1.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            var updateTemp = new TemplateDto
                            {
                                Id = response[0].Id,
                                TemplateId = tempResult.id,
                                Status = tempResult.status,
                                Category = tempResult.category,
                                ActionBy = template.ActionBy
                            };

                            var updateTemplate = await UpdateTemplateStatusByIdAsync(updateTemp);
                            if (updateTemplate == null || updateTemplate.Status <= 0)
                            {
                                return new UResponseWithID
                                {
                                    Status = 201, //Created but not created in facebook
                                    Message = "Template created in system but was not created on facebook because of: \n" + updateTemplate?.Message,
                                    Id = response[0].Id
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
                        Message = "Template created in system but was not created on facebook because of: \n" + result.message,
                        Id = response[0].Id
                    };
                }
            }
            else if (response[0].Status <= 0)
            {
                return new UResponseWithID()
                {
                    Status = 0,
                    Message = response[0].Message
                };
            }

            return new UResponseWithID
            {
                Status = 0,
                Message = "Oops somethng went wrong"
            };
        }

        public async Task<UResponseWithID> UpdateTemplateAsync(TemplateDto template)
        {
            int headerType = 0;
            var headerText = "";
            int headerParamCount = 0;
            var bodyText = "";
            int bodyParamCount = 0;
            var footer = "";
            var mediaUrl = "";
            var headerValues = new List<TemplateDto.KeyValue>();
            var bodyValues = new List<TemplateDto.KeyValue>();
            var buttonValues = new List<TemplateParameter>();
            Regex regex = new Regex(@"{{\d+}}");

            //template.Name = template.Name.Replace(" ", "_").ToLower();
            //var templateNameExist = await _dbContext.Templates.Where(x => x.TemplateName == template.Name && x.Id != template.Id).FirstOrDefaultAsync();
            //if (templateNameExist != null)
            //{
            //    return new UResponseWithID()
            //    {
            //        Status = 0,
            //        Message = "Template name already exist"
            //    };
            //}

            if (template.Header != null)
            {
                if (template.Header.Format == (int)TemplateHeaderEnum.IMAGE
                    || template.Header.Format == (int)TemplateHeaderEnum.VIDEO
                    || template.Header.Format == (int)TemplateHeaderEnum.DOCUMENT)
                {
                    if (template.MediaId <= 0)
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media Id required when header type is not text"
                        };
                    var mediaDetail = await _dbContext.Medias.Where(x => x.Id == template.MediaId).FirstOrDefaultAsync();
                    if (mediaDetail == null && string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };
                    else
                        mediaUrl = string.Concat(_apiSolutionConfigurationSettings.Value.BaseURL, mediaDetail.MediaPath);

                    if (mediaDetail.SenderNameId != template.SenderNameId)
                    {
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = "Media does not exist for this sender"
                        };
                    }

                    var allowedMedia = _mediaService.CheckAllowedTemplateHeaderType((TemplateHeaderEnum)template.Header.Format, mediaDetail.FileExtension);
                    if (!allowedMedia)
                    {
                        return new UResponseWithID
                        {
                            Status = 0,
                            Message = $"Not allowed media for header type - {(TemplateHeaderEnum)template.Header.Format}"
                        };
                    }

                }
                if (!string.IsNullOrEmpty(template.Header.Text))
                {
                    MatchCollection matches = regex.Matches(template.Header.Text);
                    if (matches.Count() > 1)
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Only one header parameters is allowed"
                        };
                    if (!(template.Header.TextCount == matches.Count))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Header text parameters is not matching with header text count"
                        };
                }
                if (template.Header.TextCount == 0)
                    template.Header.Values = null;

                headerType = template.Header.Format;
                headerText = template.Header.Text;
                headerParamCount = template.Header.TextCount;
            }
            if (template.Body != null)
            {
                MatchCollection matches = regex.Matches(template.Body.Text);
                if (!(template.Body.TextCount == matches.Count))
                    return new UResponseWithID()
                    {
                        Status = 0,
                        Message = "Body text parameters is not matching with body text count"
                    };

                if (template.Body.TextCount == 0)
                    template.Body.Values = null;

                bodyText = template.Body.Text;
                bodyParamCount = template.Body.TextCount;
            }
            if (template.Footer != null)
                footer = template.Footer.Text;

            if (template.Header != null && template.Header.Values != null && template.Header.Values.Any())
            {
                foreach (var val in template.Header.Values)
                {
                    var param = new TemplateDto.KeyValue()
                    {
                        Index = val.Index,
                        Value = val.Value,
                        DefaultValue = val.DefaultValue
                    };
                    headerValues.Add(param);
                }
            }

            if (template.Body != null && template.Body.Values != null && template.Body.Values.Any())
            {
                foreach (var val in template.Body.Values)
                {
                    var param = new TemplateDto.KeyValue()
                    {
                        Index = val.Index,
                        Value = val.Value,
                        DefaultValue = val.DefaultValue
                    };
                    bodyValues.Add(param);
                }
            }

            if (template.Buttons != null && template.Buttons.Any())
            {
                foreach (var button in template.Buttons)
                {
                    //if (button.TextCount == 0)
                    //    button.Values = null;

                    if (button.ActionType == (int)ActionTypeEnum.TEMPLATE && button.ActionId <= 0)
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Template Id required in action id when action type is template"
                        };

                    var param = new TemplateParameter()
                    {
                        Sequence = button.Index,
                        ParamName = button.Text,
                        ParamText = String.Empty,
                        ParamDefaultValue = String.Empty, //button.Url,
                        IsDynamic = false,
                        ParamType = (int)TemplateParamEnum.Button,
                        ButtonType = button.Type,
                        ActionId = button.ActionId,
                        ActionType = button.ActionType,
                        ButtonId = button.ButtonId
                    };

                    // Handle URL button cases
                    if (button.Type == (int)ButtonTypeEnum.URL)
                    {
                        //if (button.TextCount == 0)
                        //{
                        //param.ParamText = button.Url; // Use URL as text
                        //}
                        //else if (button.TextCount == 1)
                        //{
                        param.ParamText = button.Url; // Use URL as text
                        param.ParamDefaultValue = button.Values != null && !String.IsNullOrWhiteSpace(button.Values.Value) ? button.Values.Value : "";
                        param.IsDynamic = button.Values != null && !String.IsNullOrWhiteSpace(button.Values.Value); // Mark as dynamic
                        //}
                    }
                    else if (button.Type == (int)ButtonTypeEnum.PHONE_NUMBER)
                    {
                        param.ParamText = button.PhoneNumber;
                        //param.ParamDefaultValue = button.PhoneNumber;
                    }

                    buttonValues.Add(param);
                }
            }

            var headerJson = JsonSerializer.Serialize(headerValues);
            var bodyJson = JsonSerializer.Serialize(bodyValues);
            var buttonJson = JsonSerializer.Serialize(buttonValues);

            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Update}, @TemplatesId={template.Id}, @ClientId={template.ClientId}, @TemplateName={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderType={headerType}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @HeaderValues={headerJson}, @BodyValues={bodyJson}, @FooterText={footer}, @ButtonValues={buttonJson}, @TransactionType={template.TransactionType}, @MediaId={template.MediaId}, @SenderId={template.SenderNameId}, @ActionBy={template.ActionBy}, @DefaultType={template.DefaultType}").ToListAsync();
            if (response == null || !response.Any())
                return new UResponseWithID()
                {
                    Status = 0,
                    Message = "Oops somethng went wrong"
                };

            if (response[0].Status > 0)
            {
                var tempateResponse = new TemplateRequestDto()
                {
                    ClientId = template.ClientId.ToString(),
                    SenderNameId = template.SenderNameId.ToString(),
                    Name = template.Name,
                    Category = template.Category,
                    LanguageCode = template.Language,
                };

                tempateResponse.Header = new TemplateRequestDto.HeaderDto()
                {
                    Format = ((TemplateHeaderEnum)template.Header.Format).ToString(),
                    MediaUrl = mediaUrl,
                    Text = template.Header.Text,
                    Example = template.Header.Values?.FirstOrDefault()?.Value ?? ""
                };

                tempateResponse.Body = new TemplateRequestDto.BodyDto()
                {
                    Text = template.Body.Text,
                    Examples = template.Body?.Values?.Select(x => x.Value).ToList() ?? new List<string>()
                };

                tempateResponse.Footer = new TemplateRequestDto.FooterDto()
                {
                    Text = template.Footer.Text
                };

                foreach (var item in template.Buttons)
                {
                    tempateResponse.Buttons.Add(new TemplateRequestDto.ButtonDto()
                    {
                        Type = ((ButtonTypeEnum)item.Type).ToString(),
                        Text = item.Text,
                        PhoneNumber = item.PhoneNumber,
                        Url = item.Url,
                        Example = item.Values != null && !String.IsNullOrWhiteSpace(item.Values.Value) ? item.Values.Value : ""
                    });
                }

                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempateResponse), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/TemplateMessageOps", res);
                var content = await response1.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            var updateTemp = new TemplateDto()
                            {
                                Id = response[0].Id,
                                TemplateId = tempResult.id,
                                Status = tempResult.status,
                                Category = tempResult.category,
                                ActionBy = template.ActionBy
                            };

                            var updateTemplate = await UpdateTemplateStatusByIdAsync(updateTemp);
                            if (updateTemplate == null || updateTemplate.Status <= 0)
                            {
                                return new UResponseWithID()
                                {
                                    Status = 201,
                                    Message = "Template updated in system but was not updated on facebook because of: \n" + updateTemplate?.Message
                                };
                            }

                            return new UResponseWithID()
                            {
                                Status = 200,
                                Message = "Template updated successfully"
                            };
                        }
                    }
                }
                else if (result != null && !result.success)
                {
                    return new UResponseWithID()
                    {
                        Status = 201,
                        Message = "Template updated in system but was not updated on facebook because of: \n" + result.message
                    };
                }
            }
            else if (response[0].Status <= 0)
            {
                return new UResponseWithID()
                {
                    Status = 0,
                    Message = response[0].Message
                };
            }

            return new UResponseWithID()
            {
                Status = 0,
                Message = "Oops somethng went wrong"
            };
        }

        public async Task<UResponseWithID> DeleteTemplateAsync(int Id)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Delete}, @TemplatesId={Id}").ToListAsync();
            return response[0];
        }

        public async Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateDto template)
        {
            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.UpdateTemplateStatus}, @TemplatesId={template.Id}, @TemplateId={template.TemplateId}, @Status={template.Status}, @Category={template.Category}, @ActionBy={template.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UTemplateDetails> GetTemplateDetailsAsync(int client_Id, int template_Id = 0)
        {
            UTemplateDetails pDetails = null;

            var response = await _dbContext2.TemplateDetails.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.GetTemplateDetails}, @ClientId={client_Id}, @TemplatesId={template_Id}").ToListAsync();
            if (response != null && response.Any())
            {
                pDetails = response[0];
                if (pDetails.Id > 0)
                {
                    var templateParameters = await GetTemplateParametersAsync(client_Id, template_Id);
                    if (templateParameters.Any())
                    {
                        var headerValue = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                        if (headerValue != null)
                        {
                            pDetails.HeaderValue = new KeyValue 
                            {
                                Index = headerValue.Sequence,
                                Value = headerValue.ParamDefaultValue,
                                DefaultValue = headerValue.ParamDefaultValue
                            };
                        }
                        var bodyValue = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
                        if (bodyValue != null)
                        {
                            foreach (var item in bodyValue)
                            {
                                pDetails.BodyValues.Add(new KeyValue 
                                {
                                    Index = item.Sequence,
                                    Value = item.ParamDefaultValue,
                                    DefaultValue = item.ParamDefaultValue
                                });
                            }
                        }
                        var buttonValues = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
                        if (buttonValues != null)
                        {
                            foreach (var item in buttonValues)
                            {
                                var buttonValue = new ButtonValue
                                {
                                    ButtonId = item.ButtonId,
                                    Type = item.ButtonType,
                                    Text = item.ParamName,
                                    Sequence = item.Sequence ?? 0,
                                    Index = item.Sequence ?? 0,
                                    ActionId = item.ActionId,   
                                    ActionType = item.ActionType,
                                };

                                if ((ButtonTypeEnum)item.ButtonType == ButtonTypeEnum.PHONE_NUMBER)
                                    buttonValue.PhoneNumber = item.ParamText;
                                else if ((ButtonTypeEnum)item.ButtonType == ButtonTypeEnum.URL)
                                {
                                    buttonValue.IsDynamic = item.IsDynamic;
                                    buttonValue.Url = item.ParamText;
                                    if (!String.IsNullOrWhiteSpace(item.ParamDefaultValue))
                                    {
                                        buttonValue.Values = new KeyValue
                                        {
                                            Index = item.Sequence,
                                            Value = item.ParamDefaultValue,
                                            DefaultValue = item.ParamDefaultValue
                                        };
                                    }
                                }

                                pDetails.ButtonValues.Add(buttonValue);
                            }
                        }
                    }
                }
            }

            return pDetails;
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
