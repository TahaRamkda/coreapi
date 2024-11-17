using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CustomIntegrationService : ICustomIntegrationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ITemplateService _templateService;
        private readonly HttpClient _httpClient;

        public CustomIntegrationService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ITemplateService templateService,
            IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _templateService = templateService;
            _httpClient = httpClientFactory.CreateClient("bridge_api");
        }

        public async Task<UResponse> SendSmsAsync(SendSmsDto sendSms, int ClientId)
        {
            sendSms.BrandName = sendSms.BrandName.Replace(" ", "_");
            var templateName = string.Concat(sendSms.BrandName, "_", sendSms.TemplateName).ToLower();
            var templateDetails = await _templateService.GetTemplateDetailsAsync(client_Id: ClientId, searchStr: templateName);
            if (templateDetails != null)
            {
                var sendMessage = new SendTemplateMessageDto()
                {
                    ClientId = templateDetails.ClientId.ToString(),
                    SenderNameId = templateDetails.SenderId.ToString(),
                    PhoneNumbers = new List<string> { sendSms.Phone },
                    LanguageCode = templateDetails.Language,
                    TemplateId = templateDetails.TemplateId,
                    TemplateName = templateDetails.TemplateName
                };
                if (templateDetails.HeaderParamCount > 0)
                {
                    var headerComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Header.ToString()
                    };
                    headerComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                    {
                        Type = ((TemplateHeaderEnum)templateDetails.HeaderType).ToString(),
                        Value = templateDetails.HeaderValue.Value ?? templateDetails.HeaderValue.DefaultValue,
                        Index = templateDetails.HeaderValue.Index
                    });
                    sendMessage.Components.Add(headerComponents);
                }
                if (templateDetails.BodyParamCount > 0)
                {
                    var bodyComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Body.ToString()
                    };
                    for (int i = 0; i <= templateDetails.BodyValues.Count(); i++)
                    {
                        if (i == 0)
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.OrderId,
                                Index = i + 1
                            });
                        }
                        if (i == 1)
                        {
                            bodyComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                            {
                                Type = "text",
                                Value = sendSms.Amount.ToString(),
                                Index = i + 1
                            });
                        }
                    }
                    sendMessage.Components.Add(bodyComponents);
                }
                if (templateDetails.ButtonValues.Any())
                {
                    var buttonComponents = new SendTemplateMessageDto.TemplateComponent()
                    {
                        ComponentType = TemplateParamEnum.Button.ToString()
                    };
                    foreach (var button in templateDetails.ButtonValues)
                    {
                        buttonComponents.Values.Add(new SendTemplateMessageDto.TemplateKeyValue()
                        {
                            Type = button.Type,
                            Value = sendSms.OrderId,
                            Index = button.Index
                        });
                    }
                    sendMessage.Components.Add(buttonComponents);
                }
                var res = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(sendMessage), Encoding.UTF8, "application/json");
                var response1 = await _httpClient.PostAsync($"/api/Template/SendBatchTemplateMessage", res);
                var content = await response1.Content.ReadAsStringAsync();

                var result = System.Text.Json.JsonSerializer.Deserialize<SyncResultDto>(content);
                if (result != null && result.success)
                {
                    var data = System.Text.Json.JsonSerializer.Serialize(result.result);
                    var tempResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SendSmsResultDto>(data);
                    if (tempResult != null)
                    {
                        if (tempResult.Success)
                        {

                        }
                    }
                }
                else if (result != null && !result.success)
                {
                    return new UResponse()
                    {
                        Status = 0,
                        Message = result.message
                    };
                }
            }

            var query = "";// string.Format(@"exec usp_Clients_Ops @ActionId={0}, @Client_Name='{1}', @Client_Language={2}, @Client_Address='{3}', @Balance={4}, @Contact_Person='{5}', @Contact_Person_Email='{6}', @Contact_Person_Phone='{7}', @Balance_Alert_Limit={8}, @Access_Token='{9}', @Action_By={10}", (int)CrudEnum.Add, client.Client_Name, client.Client_Language, client.Client_Address, client.Balance, client.Contact_Person, client.Contact_Person_Email, client.Contact_Person_Phone, client.Balance_Alert_Limit, client.Access_Token, client.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
