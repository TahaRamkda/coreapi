using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Data;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using static WhatsAppAPISolutionDL.Dto.MediaUploadBridgeDto;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly HttpClient _httpClient;

        public TemplateService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
          IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpClient = httpClientFactory.CreateClient("bridge_api");
        }

        public async Task<List<UTemplate>> GetTemplateListAsync(int client_Id)
        {
            var query = string.Format(@"exec usp_Templates_Ops_Bak @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, client_Id);
            var response = await _dbContext2.Templates.FromSqlRaw(query).ToListAsync();

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
            template.Name = template.Name.Replace(" ", "_").ToLower();
            var templateNameExist = await _dbContext.Templates.Where(x => x.TemplateName == template.Name).FirstOrDefaultAsync();
            if (templateNameExist != null)
            {
                return new UResponseWithID()
                {
                    Status = 0,
                    Message = "Template name already exist"
                };
            }
            if (template.Header != null)
            {
                if (template.Header.Format != (int)TemplateHeaderEnum.TEXT)
                {
                    if (string.IsNullOrEmpty(template.MediaId))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media Id required when header type is not text"
                        };
                    var mediaDetail = await _dbContext.Medias.Where(x => x.MediaId == template.MediaId).FirstOrDefaultAsync();
                    if (mediaDetail == null && string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };
                    else
                        mediaUrl = mediaDetail.MediaPath;
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
                        index = val.index,
                        value = val.value,
                        defaultValue = val.defaultValue
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
                        index = val.index,
                        value = val.value,
                        defaultValue = val.defaultValue
                    };
                    bodyValues.Add(param);
                }
            }
            if (template.Buttons != null && template.Buttons.Any())
            {
                foreach (var button in template.Buttons)
                {
                    if (button.TextCount == 0)
                        button.Values = null;
                    if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                    {
                        var param = new TemplateParameter()
                        {
                            Sequence = button.index,
                            ParamName = button.Text,
                            ParamText = string.Empty,
                            ParamDefaultValue = button.Url,
                            IsDynamic = false,
                            ParamType = (int)TemplateParamEnum.Button,
                            ButtonType = (int)parsedEnum
                        };

                        // Handle URL button cases
                        if (parsedEnum == ButtonTypeEnum.URL)
                        {
                            if (button.TextCount == 0)
                            {
                                param.ParamText = button.Url; // Use URL as text
                            }
                            else if (button.TextCount == 1)
                            {
                                param.ParamText = button.Url; // Use URL as text
                                param.ParamDefaultValue = button.Values.Any() ? button.Values[0].value : "";
                                param.IsDynamic = true; // Mark as dynamic
                            }
                        }
                        else if (parsedEnum == ButtonTypeEnum.PHONE_NUMBER)
                        {
                            param.ParamDefaultValue = button.PhoneNumber;
                        }
                        buttonValues.Add(param);
                    }
                }
            }

            var headerJson = JsonSerializer.Serialize(headerValues);
            var bodyJson = JsonSerializer.Serialize(bodyValues);
            var buttonJson = JsonSerializer.Serialize(buttonValues);

            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops_Bak @ActionId={(int)CrudEnum.Add}, @ClientId={template.Client_Id}, @TemplateName={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderType={headerType}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @HeaderValues={headerJson}, @BodyValues={bodyJson}, @FooterText={footer}, @ButtonValues={buttonJson}, @TransactionType={template.Template_Type}, @MediaId={template.MediaId}, @SenderId={template.Sender_Name_Id}, @Action_By={template.ActionBy}").ToListAsync();
            if (response != null || response[0].Status > 0)
            {
                var tempateResponse = new TemplateRequestDto()
                {
                    ClientId = template.Client_Id.ToString(),
                    SenderNameId = template.Sender_Name_Id.ToString(),
                    Name = template.Name,
                    Category = template.Category,
                    LanguageCode = template.Language,
                };
                tempateResponse.Header = new TemplateRequestDto.HeaderDto()
                {
                    Format = ((TemplateHeaderEnum)template.Header.Format).ToString(),
                    MediaUrl = mediaUrl,
                    Text = template.Header.Text,
                    Example = template.Header.Values?.FirstOrDefault()?.value ?? ""
                };
                tempateResponse.Body = new TemplateRequestDto.BodyDto()
                {
                    Text = template.Body.Text,
                    Examples = template.Body?.Values?.Select(x => x.value).ToList() ?? new List<string>()
                };
                tempateResponse.Footer = new TemplateRequestDto.FooterDto()
                {
                    Text = template.Footer.Text
                };
                foreach (var item in template.Buttons)
                {
                    tempateResponse.Buttons.Add(new TemplateRequestDto.ButtonDto()
                    {
                        Type = item.Type,
                        Text = item.Text,
                        PhoneNumber = item.PhoneNumber,
                        Url = item.Url,
                        Example = item.Values?.FirstOrDefault()?.value,
                    });
                }
                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempateResponse), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/TemplateMessageOps", res);
                var content = await response1.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            var updateTemp = new TemplateDto()
                            {
                                Templates_Id = response[0].Id,
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
                                    Status = 0,
                                    Message = updateTemplate?.Message
                                };
                            }
                            return new UResponseWithID()
                            {
                                Status = 1,
                                Message = "Template added successfully"
                            };
                        }
                    }
                }
                else if (result != null && !result.success)
                {
                    return new UResponseWithID()
                    {
                        Status = 0,
                        Message = result.message
                    };
                }
            }
            return new UResponseWithID()
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

            template.Name = template.Name.Replace(" ", "_");
            var templateNameExist = await _dbContext.Templates.Where(x => x.TemplateName == template.Name && x.TemplatesId != template.Templates_Id).FirstOrDefaultAsync();
            if (templateNameExist != null)
            {
                return new UResponseWithID()
                {
                    Status = 0,
                    Message = "Template name already exist"
                };
            }
            if (template.Header != null)
            {
                if (template.Header.Format != (int)TemplateHeaderEnum.NONE && template.Header.Format != (int)TemplateHeaderEnum.TEXT)
                {
                    if (string.IsNullOrEmpty(template.MediaId))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media Id required when header type is not text"
                        };
                    var mediaDetail = await _dbContext.Medias.Where(x => x.MediaId == template.MediaId).FirstOrDefaultAsync();
                    if (mediaDetail == null && string.IsNullOrEmpty(mediaDetail.MediaPath))
                        return new UResponseWithID()
                        {
                            Status = 0,
                            Message = "Media not exist"
                        };
                    else
                        mediaUrl = mediaDetail.MediaPath;
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

                bodyText = template.Body.Text;
                bodyParamCount = template.Body.TextCount;
            }
            if (template.Footer != null)
                footer = template.Footer.Text;

            if (template.Header != null && template.Header.Values.Any())
            {
                foreach (var val in template.Header.Values)
                {
                    var param = new TemplateDto.KeyValue()
                    {
                        index = val.index,
                        value = val.value,
                        defaultValue = val.defaultValue
                    };
                    headerValues.Add(param);
                }
            }

            if (template.Body != null && template.Body.Values.Any())
            {
                foreach (var val in template.Body.Values)
                {
                    var param = new TemplateDto.KeyValue()
                    {
                        index = val.index,
                        value = val.value,
                        defaultValue = val.defaultValue
                    };
                    bodyValues.Add(param);
                }
            }
            if (template.Buttons != null && template.Buttons.Any())
            {
                foreach (var button in template.Buttons)
                {
                    if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                    {
                        var param = new TemplateParameter()
                        {
                            Sequence = button.index,
                            ParamName = button.Text,
                            ParamText = string.Empty,
                            ParamDefaultValue = button.Url,
                            IsDynamic = false,
                            ParamType = (int)TemplateParamEnum.Button,
                            ButtonType = (int)parsedEnum
                        };

                        // Handle URL button cases
                        if (parsedEnum == ButtonTypeEnum.URL)
                        {
                            if (button.TextCount == 0)
                            {
                                param.ParamText = button.Url; // Use URL as text
                            }
                            else if (button.TextCount == 1)
                            {
                                param.ParamText = button.Url; // Use URL as text
                                param.ParamDefaultValue = button.Values.Any() ? button.Values[0].value : "";
                                param.IsDynamic = true; // Mark as dynamic
                            }
                        }
                        else if (parsedEnum == ButtonTypeEnum.PHONE_NUMBER)
                        {
                            param.ParamDefaultValue = button.PhoneNumber;
                        }
                        buttonValues.Add(param);
                    }
                }
            }

            var headerJson = JsonSerializer.Serialize(headerValues);
            var bodyJson = JsonSerializer.Serialize(bodyValues);
            var buttonJson = JsonSerializer.Serialize(buttonValues);

            var response = await _dbContext2.ResponseWithID.FromSqlInterpolated($"exec usp_Templates_Ops_Bak @ActionId={(int)CrudEnum.Update}, @TemplatesId={template.Templates_Id}, @ClientId={template.Client_Id}, @TemplateName={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderType={headerType}, @HeaderParamCount={headerParamCount}, @HeaderText={headerText}, @BodyText={bodyText}, @BodyParamCount={bodyParamCount}, @HeaderValues={headerJson}, @BodyValues={bodyJson}, @FooterText={footer}, @ButtonValues={buttonJson}, @TransactionType={template.Template_Type}, @MediaId={template.MediaId}, @SenderId={template.Sender_Name_Id}, @Action_By={template.ActionBy}").ToListAsync();
            if (response != null || response[0].Status > 0)
            {
                var tempateResponse = new TemplateRequestDto()
                {
                    ClientId = template.Client_Id.ToString(),
                    SenderNameId = template.Sender_Name_Id.ToString(),
                    Name = template.Name,
                    Category = template.Category,
                    LanguageCode = template.Language,
                };
                tempateResponse.Header = new TemplateRequestDto.HeaderDto()
                {
                    Format = ((TemplateHeaderEnum)template.Header.Format).ToString(),
                    MediaUrl = mediaUrl,
                    Text = template.Header.Text,
                    Example = template.Header.Values.Any() ? template.Header.Values[0].value : string.Empty
                };
                tempateResponse.Body = new TemplateRequestDto.BodyDto()
                {
                    Text = template.Body.Text,
                    Examples = template.Body.Values.Select(x => x.value).ToList()
                };
                tempateResponse.Footer = new TemplateRequestDto.FooterDto()
                {
                    Text = template.Footer.Text
                };
                foreach (var item in template.Buttons)
                {
                    tempateResponse.Buttons.Add(new TemplateRequestDto.ButtonDto()
                    {
                        Type = item.Type,
                        Text = item.Text,
                        PhoneNumber = item.PhoneNumber,
                        Url = item.Url,
                        Example = item.Values[0].value,
                    });
                }
                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempateResponse), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/TemplateMessageOps", res);
                var content = await response1.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateResultDto>(data);
                    if (tempResult != null)
                    {
                        if (!string.IsNullOrEmpty(tempResult.id) && !string.IsNullOrEmpty(tempResult.status))
                        {
                            var updateTemp = new TemplateDto()
                            {
                                Templates_Id = response[0].Id,
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
                                    Status = 0,
                                    Message = updateTemplate?.Message
                                };
                            }
                            return new UResponseWithID()
                            {
                                Status = 1,
                                Message = "Template updated successfully"
                            };
                        }
                    }
                }
            }
            return new UResponseWithID()
            {
                Status = 0,
                Message = "Oops somethng went wrong"
            };
        }
        public async Task<UResponseWithID> DeleteTemplateAsync(int templates_Id)
        {
            var query = string.Format(@"exec usp_Templates_Ops_Bak @ActionId={0}, @TemplatesId={1}", (int)CrudEnum.Delete, templates_Id);
            var response = await _dbContext2.ResponseWithID.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> AddTemplateWithParameterAsync(TemplateWithParametersDto templateWithParam)
        {
            if (templateWithParam == null) throw new ArgumentNullException(nameof(templateWithParam));
            var response = new UResponse();
            var templates = _dbContext.Templates.Where(x => x.TemplateId == templateWithParam.Id).ToList();
            if (!templates.Any())
            {
                var insertTemp = new Template()
                {
                    TemplateId = templateWithParam.Id,
                    TemplateName = templateWithParam.Name,
                    Category = templateWithParam.Category,
                    SubCategory = templateWithParam.SubCategory,
                    Language = templateWithParam.Language,
                    Status = templateWithParam.Status,
                    IsApproved = templateWithParam.IsApproved,
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                };

                if (templateWithParam.Header != null)
                {
                    int enumValue = 1;
                    if (Enum.TryParse(templateWithParam.Header.Format, true, out TemplateHeaderEnum parsedEnum))
                    {
                        enumValue = (int)parsedEnum; // Get the integer value
                    }

                    insertTemp.HeaderType = enumValue;
                    insertTemp.HeaderText = templateWithParam.Header.Text;
                    insertTemp.HeaderParamCount = templateWithParam.Header.TextCount;
                }
                if (templateWithParam.Body != null)
                {
                    insertTemp.BodyText = templateWithParam.Body.Text;
                    insertTemp.BodyParamCount = templateWithParam.Body.TextCount;
                }
                if (templateWithParam.Footer != null)
                {
                    insertTemp.FooterText = templateWithParam.Footer.Text;
                }

                _dbContext.Templates.Add(insertTemp);
                await _dbContext.SaveChangesAsync();

                if (templateWithParam.Header != null && templateWithParam.Header.Values.Any())
                {
                    foreach (var val in templateWithParam.Header.Values)
                    {
                        var param = new TemplateParameter()
                        {
                            TemplatesId = insertTemp.TemplatesId,
                            Sequence = val.index,
                            ParamName = string.Empty,
                            ParamText = string.Empty,
                            ParamDefaultValue = val.value,
                            ParamType = (int)TemplateParamEnum.Header,
                        };
                        _dbContext.TemplateParameters.Add(param);
                    }
                }
                if (templateWithParam.Body != null && templateWithParam.Body.Values.Any())
                {
                    foreach (var val in templateWithParam.Body.Values)
                    {
                        var param = new TemplateParameter()
                        {
                            TemplatesId = insertTemp.TemplatesId,
                            Sequence = val.index,
                            ParamName = string.Empty,
                            ParamText = string.Empty,
                            ParamDefaultValue = val.value,
                            ParamType = (int)TemplateParamEnum.Body,
                        };
                        _dbContext.TemplateParameters.Add(param);
                    }
                }
                if (templateWithParam.Buttons != null && templateWithParam.Buttons.Any())
                {
                    foreach (var button in templateWithParam.Buttons)
                    {
                        if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                        {
                            var param = new TemplateParameter()
                            {
                                TemplatesId = insertTemp.TemplatesId,
                                Sequence = button.index,
                                ParamName = button.Text,
                                ParamText = string.Empty,
                                ParamDefaultValue = button.Url,
                                IsDynamic = false,
                                ParamType = (int)TemplateParamEnum.Button,
                            };

                            // Handle URL button cases
                            if (parsedEnum == ButtonTypeEnum.URL)
                            {
                                if (button.TextCount == 0)
                                {
                                    param.ParamText = button.Url; // Use URL as text
                                }
                                else if (button.TextCount == 1)
                                {
                                    param.ParamText = button.Url; // Use URL as text
                                    param.ParamDefaultValue = button.Values.Any() ? button.Values[0].value : "";
                                    param.IsDynamic = true; // Mark as dynamic
                                }
                            }
                            else if (parsedEnum == ButtonTypeEnum.PHONE_NUMBER)
                            {
                                param.ParamDefaultValue = button.PhoneNumber;
                            }

                            _dbContext.TemplateParameters.Add(param);
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
                response = new UResponse()
                {
                    Status = 1,
                    Message = "Template added successfully"
                };
            }
            else
            {
                // Assuming templates is a collection of templates and we have the templateId
                var existingTemplate = templates.FirstOrDefault(t => t.TemplateId == templateWithParam.Id);

                if (existingTemplate != null)
                {
                    // Update properties of the existing template
                    existingTemplate.TemplateName = templateWithParam.Name;
                    existingTemplate.Category = templateWithParam.Category;
                    existingTemplate.SubCategory = templateWithParam.SubCategory;
                    existingTemplate.Language = templateWithParam.Language;
                    existingTemplate.Status = templateWithParam.Status;
                    existingTemplate.IsApproved = templateWithParam.IsApproved;
                    existingTemplate.UpdatedBy = 1;
                    existingTemplate.UpdatedDate = DateTime.UtcNow;

                    if (templateWithParam.Header != null)
                    {
                        int enumValue = 1;
                        if (Enum.TryParse(templateWithParam.Header.Format, true, out TemplateHeaderEnum parsedEnum))
                        {
                            enumValue = (int)parsedEnum; // Get the integer value
                        }

                        existingTemplate.HeaderType = enumValue;
                        existingTemplate.HeaderText = templateWithParam.Header.Text;
                        existingTemplate.HeaderParamCount = templateWithParam.Header.TextCount;
                    }

                    if (templateWithParam.Body != null)
                    {
                        existingTemplate.BodyText = templateWithParam.Body.Text;
                        existingTemplate.BodyParamCount = templateWithParam.Body.TextCount;
                    }

                    if (templateWithParam.Footer != null)
                    {
                        existingTemplate.FooterText = templateWithParam.Footer.Text;
                    }

                    // Remove existing parameters for this template
                    var existingParameters = _dbContext.TemplateParameters.Where(p => p.TemplatesId == existingTemplate.TemplatesId).ToList();
                    _dbContext.TemplateParameters.RemoveRange(existingParameters);

                    // Add new parameters for Header
                    if (templateWithParam.Header != null && templateWithParam.Header.Values.Any())
                    {
                        foreach (var val in templateWithParam.Header.Values)
                        {
                            var param = new TemplateParameter()
                            {
                                TemplatesId = existingTemplate.TemplatesId,
                                Sequence = val.index,
                                ParamName = string.Empty,
                                ParamText = string.Empty,
                                ParamDefaultValue = val.value,
                                ParamType = (int)TemplateParamEnum.Header,
                            };
                            _dbContext.TemplateParameters.Add(param);
                        }
                    }

                    // Add new parameters for Body
                    if (templateWithParam.Body != null && templateWithParam.Body.Values.Any())
                    {
                        foreach (var val in templateWithParam.Body.Values)
                        {
                            var param = new TemplateParameter()
                            {
                                TemplatesId = existingTemplate.TemplatesId,
                                Sequence = val.index,
                                ParamName = string.Empty,
                                ParamText = string.Empty,
                                ParamDefaultValue = val.value,
                                ParamType = (int)TemplateParamEnum.Body,
                            };
                            _dbContext.TemplateParameters.Add(param);
                        }
                    }
                    if (templateWithParam.Buttons != null && templateWithParam.Buttons.Any())
                    {
                        foreach (var button in templateWithParam.Buttons)
                        {
                            if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                            {
                                var param = new TemplateParameter()
                                {
                                    TemplatesId = existingTemplate.TemplatesId,
                                    Sequence = 1,
                                    ParamName = button.Text,
                                    ParamText = string.Empty,
                                    ParamDefaultValue = button.Url,
                                    IsDynamic = false,
                                    ParamType = (int)TemplateParamEnum.Button,
                                };

                                // Handle URL button cases
                                if (parsedEnum == ButtonTypeEnum.URL)
                                {
                                    if (button.TextCount == 0)
                                    {
                                        param.ParamText = button.Url; // Use URL as text
                                    }
                                    else if (button.TextCount == 1)
                                    {
                                        param.ParamText = button.Url; // Use URL as text
                                        param.ParamDefaultValue = button.Values.Any() ? button.Values[0].value : "";
                                        param.IsDynamic = true; // Mark as dynamic
                                    }
                                }
                                else if (parsedEnum == ButtonTypeEnum.PHONE_NUMBER)
                                {
                                    param.ParamDefaultValue = button.PhoneNumber;
                                }

                                _dbContext.TemplateParameters.Add(param);
                            }
                        }
                    }

                    // Save all changes in a single transaction
                    await _dbContext.SaveChangesAsync();
                    response = new UResponse()
                    {
                        Status = 1,
                        Message = "Template updated successfully"
                    };
                }
            }
            return response;
        }
        public async Task<UResponseWithID> UpdateTemplateStatusByIdAsync(TemplateDto template)
        {
            var query = string.Format(@"exec usp_Templates_Ops_Bak @ActionId={0}, @TemplatesId={1}, @TemplateId='{2}', @Status='{3}', @Category='{4}', @Action_By={5}", (int)CrudEnum.UpdateTemplateStatus, template.Templates_Id, template.TemplateId, template.Status, template.Category, template.ActionBy);
            var response = await _dbContext2.ResponseWithID.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UTemplateDetails> GetTemplateDetailsAsync(int client_Id, int templates_Id = 0, string searchStr = "")
        {
            UTemplateDetails pDetails = null;
            var query = string.Format(@"exec usp_Templates_Ops_Bak @ActionId={0}, @ClientId={1}, @TemplatesId={2}", (int)CrudEnum.GetTemplateDetails, client_Id, templates_Id);
            var response = await _dbContext2.TemplateDetails.FromSqlRaw(query).ToListAsync();
            if (response != null && response.Any())
            {
                pDetails = response[0];
                if (pDetails.TemplatesId > 0)
                {
                    var templateParameters = await GetTemplateParametersAsync(client_Id, templates_Id);
                    if (templateParameters.Any())
                    {
                        var headerValue = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Header).FirstOrDefault();
                        if (headerValue != null)
                        {
                            pDetails.HeaderValue = new KeyValue()
                            {
                                Index = headerValue.Sequence,
                                Value = headerValue.ParamName,
                                DefaultValue = headerValue.ParamDefaultValue
                            };
                        }
                        var bodyValue = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Body).ToList();
                        if (bodyValue != null)
                        {
                            foreach (var item in bodyValue)
                            {
                                pDetails.BodyValues.Add(new KeyValue()
                                {
                                    Index = item.Sequence,
                                    Value = item.ParamName,
                                    DefaultValue = item.ParamDefaultValue
                                });
                            }
                        }
                        var buttonValues = templateParameters.Where(x => x.ParamType == (int)TemplateParamEnum.Button).ToList();
                        if (buttonValues != null)
                        {
                            foreach (var item in buttonValues)
                            {
                                var buttonValue = new ButtonValue()
                                {
                                    Type = ((ButtonTypeEnum)item.ButtonType).ToString(),
                                    Text = item.ParamName,
                                    PhoneNumber = item.ParamDefaultValue,
                                    Url = item.ParamName,
                                    IsDynamic = item.IsDynamic,
                                    Sequence = item.Sequence
                                };
                                buttonValue.Values.Value = item.ParamDefaultValue;
                                pDetails.ButtonValues.Add(buttonValue);
                            }
                        }
                    }
                }
            }

            return pDetails;
        }
        public async Task<List<UTemplateParameter>> GetTemplateParametersAsync(int client_Id, int templates_Id = 0)
        {
            var query = string.Format(@"exec usp_Templates_Ops_Bak @ActionId={0}, @ClientId={1}, @TemplatesId={2}", (int)CrudEnum.GetTemplateParameterDetails, client_Id, templates_Id);
            var response = await _dbContext2.TemplateParameters.FromSqlRaw(query).ToListAsync();

            return response;
        }
    }
}
