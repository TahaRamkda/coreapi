using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using static WhatsAppAPISolutionDL.Dto.SendTemplateMessageDto;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class CampaignsController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CampaignsController> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = String.Empty;
        private readonly ICampaignService _campaignService;

        public CampaignsController(ITemplateService templateService,
            WhatsAppSolutionContext dbContext,
            ILogger<CampaignsController> logger,
          IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
          IHttpClientFactory httpClientFactory,
          ICampaignService campaignService)
        {
            _templateService = templateService;
            _dbContext = dbContext;
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;
            _campaignService = campaignService;
        }

        [HttpPost("sendtemplate")]
        public async Task<IActionResult> SendTemplateMessage([FromBody] SendTemplateMessageDto sendTemplateMessage)
        {
            if (sendTemplateMessage == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(sendTemplateMessage.Phone_Id))
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "PhoneId is required"
                });
            }

            if (string.IsNullOrEmpty(sendTemplateMessage.Template_Id))
            {
                return Ok(new ApiResult
                {
                    Success = false,
                    Message = "TemplateId is required"
                });
            }


            var model = new SendTemplateMessageDto();
            model.Phone_Id = sendTemplateMessage.Phone_Id;
            model.Phone_Numbers = sendTemplateMessage.Phone_Numbers;
            model.Language_Code = sendTemplateMessage.Language_Code;
            model.Template_Id = sendTemplateMessage.Template_Id;
            model.Template_Name = sendTemplateMessage.Template_Name;

            var templateData = _dbContext.Templates.Where(x => x.TemplateId == sendTemplateMessage.Template_Id).FirstOrDefault();
            if (templateData != null)
            {
                var tempParam = _dbContext.TemplateParameters.Where(x => x.TemplatesId == templateData.TemplatesId && x.ParamType == (int)TemplateParamEnum.Header).OrderBy(y => y.ParamType).ThenBy(z => z.Sequence).ToList();
                if (tempParam.Any())
                {
                    var comp = new TemplateComponent();
                    foreach (var item in tempParam)
                    {
                        comp.Component_Type = ((TemplateParamEnum)item.ParamType).ToString();

                        var val = new TemplateKeyValue();
                        if (item.ParamType == (int)TemplateParamEnum.Header)
                            val.Type = ((TemplateHeaderEnum)templateData.HeaderType).ToString();

                        val.Type = TemplateHeaderEnum.TEXT.ToString();
                        val.Value = item.ParamDefaultValue;
                        val.Index = item.Sequence.Value;
                        comp.Values.Add(val);
                    }
                    model.Components.Add(comp);
                }
                var tempParam1 = _dbContext.TemplateParameters.Where(x => x.TemplatesId == templateData.TemplatesId && x.ParamType == (int)TemplateParamEnum.Body).OrderBy(y => y.ParamType).ThenBy(z => z.Sequence).ToList();
                if (tempParam1.Any())
                {
                    var comp = new TemplateComponent();
                    foreach (var item in tempParam1)
                    {
                        comp.Component_Type = ((TemplateParamEnum)item.ParamType).ToString();

                        var val = new TemplateKeyValue();
                        if (item.ParamType == (int)TemplateParamEnum.Header)
                            val.Type = ((TemplateHeaderEnum)templateData.HeaderType).ToString();

                        val.Type = TemplateHeaderEnum.TEXT.ToString();
                        val.Value = item.ParamDefaultValue;
                        val.Index = item.Sequence.Value;
                        comp.Values.Add(val);
                    }
                    model.Components.Add(comp);
                }
                var tempParam2 = _dbContext.TemplateParameters.Where(x => x.TemplatesId == templateData.TemplatesId && x.ParamType == (int)TemplateParamEnum.Button).OrderBy(y => y.ParamType).ThenBy(z => z.Sequence).ToList();
                if (tempParam2.Any())
                {
                    var comp = new TemplateComponent();
                    foreach (var item in tempParam2)
                    {
                        comp.Component_Type = ((TemplateParamEnum)item.ParamType).ToString();

                        var val = new TemplateKeyValue();
                        if (item.ParamType == (int)TemplateParamEnum.Header)
                            val.Type = ((TemplateHeaderEnum)templateData.HeaderType).ToString();

                        val.Type = ((ButtonTypeEnum)item.ParamType).ToString();
                        val.Value = item.ParamDefaultValue;
                        val.Index = item.Sequence.Value;
                        comp.Values.Add(val);
                    }
                    model.Components.Add(comp);
                }
            }

            var requestStr = JsonConvert.SerializeObject(model);
            var url = $"/api/template/SendBatchTemplateMessage";


            var response = await _httpClient.PostAsync($"/template/SendBatchTemplateMessage", new StringContent(requestStr, null, "application/json"));
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
                Success = true,
                Result = "",
                Message = "Data added successfully"
            });
        }

        [HttpPost("sendcampaign")]
        public async Task<IActionResult> SendCampaignMessage([FromBody] SendCampaignDto sendCampaign)
        {
            if (sendCampaign == null) return BadRequest();

            if (sendCampaign.Sender_Id == 0)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Sender Id required"
                });

            if (sendCampaign.Template_Id == 0)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Template Id required"
                });

            if (sendCampaign.Group_Id == 0)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Group Id required"
                });

            var group = await _dbContext.Groups.Where(x => x.GroupId == sendCampaign.Group_Id).FirstOrDefaultAsync();
            if (group == null)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Group not exist"
                });

            var contacts = await _dbContext.Contacts.Where(x => x.GroupId == group.GroupId).Select(x => x.PhoneNumber).ToListAsync();
            if (!contacts.Any())
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "No numbers exist in this group"
                });

            var sender = await _dbContext.SenderNames.Where(x => x.SenderId == sendCampaign.Sender_Id).FirstOrDefaultAsync();
            if (sender == null)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Sender not exist"
                });

            var template = await _dbContext.Templates.Where(x => x.TemplatesId == sendCampaign.Template_Id).FirstOrDefaultAsync();
            if (template == null)
                return Ok(new ApiResult()
                {
                    Success = false,
                    Message = "Template not exist"
                });

            var response = await _campaignService.SendCampaignAsync(sendCampaign);
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
    }
}
