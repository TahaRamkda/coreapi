using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Client_Id={1}, @Template_Name='{2}', @Integration_Id='{3}', @Template_Id='{4}', @Status={5}, @Template_Type={6}, @Action_By={7}", (int)CrudEnum.Add, template.Client_Id, template.Template_Name, template.Integration_Id, template.Template_Id, template.Status, template.Template_Type, template.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateTemplateAsync(TemplateDto template)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}, @Client_Id={2}, @Template_Name='{3}', @Integration_Id='{4}', @Template_Id='{5}', @Status={6}, @Template_Type={7}, @Action_By={8}", (int)CrudEnum.Update, template.Templates_Id, template.Client_Id, template.Template_Name, template.Integration_Id, template.Template_Id, template.Status, template.Template_Type, template.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteTemplateAsync(int template_Id)
        {
            var query = string.Format(@"exec usp_Templates_Ops @ActionId={0}, @Templates_Id={1}", (int)CrudEnum.Delete, template_Id);
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
                    IsApproved = templateWithParam.IsApproved
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
                        //int btnTypeEnum = 1;
                        //if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                        //{
                        //    btnTypeEnum = (int)parsedEnum; // Get the integer value
                        //}

                        //if (parsedEnum != ButtonTypeEnum.URL)
                        //{
                        //    var param = new TemplateParameter()
                        //    {
                        //        TemplatesId = insertTemp.TemplatesId,
                        //        Sequence = 1,
                        //        ParamName = button.Text,
                        //        ParamText = string.Empty,
                        //        ParamDefaultValue = button.Url,
                        //        IsDynamic = false,
                        //        ParamType = (int)TemplateParamEnum.Button,
                        //    };
                        //    _dbContext.TemplateParameters.Add(param);
                        //}
                        //else if (parsedEnum == ButtonTypeEnum.URL && button.TextCount == 0)
                        //{
                        //    var param = new TemplateParameter()
                        //    {
                        //        TemplatesId = insertTemp.TemplatesId,
                        //        Sequence = 1,
                        //        ParamName = button.Text,
                        //        ParamText = button.Url,
                        //        ParamDefaultValue = "",
                        //        IsDynamic = false,
                        //        ParamType = (int)TemplateParamEnum.Button,
                        //    };
                        //    _dbContext.TemplateParameters.Add(param);
                        //}
                        //else if (parsedEnum == ButtonTypeEnum.URL && button.TextCount == 1)
                        //{
                        //    var param = new TemplateParameter()
                        //    {
                        //        TemplatesId = insertTemp.TemplatesId,
                        //        Sequence = 1,
                        //        ParamName = button.Text,
                        //        ParamText = button.Url,
                        //        ParamDefaultValue = button.Values.Any() ? button.Values[0].value : "",
                        //        IsDynamic = true,
                        //        ParamType = (int)TemplateParamEnum.Button,
                        //    };
                        //    _dbContext.TemplateParameters.Add(param);
                        //}




                        if (Enum.TryParse(button.Type, true, out ButtonTypeEnum parsedEnum))
                        {
                            var param = new TemplateParameter()
                            {
                                TemplatesId = insertTemp.TemplatesId,
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
                            else if(parsedEnum == ButtonTypeEnum.PHONE_NUMBER)
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
                            //var param = new TemplateParameter()
                            //{
                            //    TemplatesId = existingTemplate.TemplatesId,
                            //    Sequence = 1,
                            //    ParamName = string.Empty,
                            //    ParamText = button.Text,
                            //    ParamDefaultValue = button.Url,
                            //    IsDynamic = false,
                            //    ParamType = (int)TemplateParamEnum.Button,
                            //};
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
