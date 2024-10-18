using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public TemplateService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UTemplate>> GetTemplateListAsync()
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}", (int)CrudEnum.List);
            var response = await _dbContext2.Templates.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddTemplateAsync(TemplateDto template)
        {
            var headerJson = JsonSerializer.Serialize(template.Header);
            var bodyJson = JsonSerializer.Serialize(template.Body);
            var buttonJson = JsonSerializer.Serialize(template.Buttons);

            var footer = "";
            if (template.Footer != null)
                footer = template.Footer.Text;

            //var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Client_Id={1}, @Template_Id='{2}', @Template_Name='{3}', @Category='{4}', SubCategory='{5}', Language='{6}', @Status='{7}', IsApproved={8}, @HeaderJson={headerJson}, @Template_Type={9}, @Action_By={10}", (int)CrudEnum.Add, template.Client_Id, template.Id, template.Name, template.Category, template.SubCategory, template.Language, template.Status, template.IsApproved, template.Template_Type, template.ActionBy);
            //var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Add}, @Client_Id={template.Client_Id}, @Template_Id={template.Id}, @Template_Name={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderJson={headerJson}, @BodyJson={bodyJson}, @Footer={footer}, @ButtonJson={buttonJson}, @Template_Type={template.Template_Type}, @Action_By={template.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateTemplateAsync(TemplateDto template)
        {
            var headerJson = JsonSerializer.Serialize(template.Header);
            var bodyJson = JsonSerializer.Serialize(template.Body);
            var buttonJson = JsonSerializer.Serialize(template.Buttons);

            var footer = "";
            if (template.Footer != null)
                footer = template.Footer.Text;

            //var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}, @Client_Id={2}, @Template_Name='{3}', @Integration_Id='{4}', @Template_Id='{5}', @Status={6}, @Template_Type={7}, @Action_By={8}", (int)CrudEnum.Update, template.Templates_Id, template.Client_Id, template.Template_Name, template.Integration_Id, template.Template_Id, template.Status, template.Template_Type, template.ActionBy);
            //var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Templates_Ops @ActionId={(int)CrudEnum.Update}, Templates_Id={template.Templates_Id}, @Client_Id={template.Client_Id}, @Template_Id={template.Id}, @Template_Name={template.Name},@Category={template.Category}, @SubCategory={template.SubCategory}, @Language={template.Language}, @Status={template.Status}, @IsApproved={template.IsApproved}, @HeaderJson={headerJson}, @BodyJson={bodyJson}, @Footer={footer}, @ButtonJson={buttonJson}, @Template_Type={template.Template_Type}, @Action_By={template.ActionBy}").ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteTemplateAsync(int templates_Id)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}", (int)CrudEnum.Delete, templates_Id);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

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
    }
}
