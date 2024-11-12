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
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CampaignsController> _logger;
        private readonly IOptions<BridgeConfigurationSettings> _bridgeConfigurationSettings;
        private readonly HttpClient _httpClient;
        private readonly string baseUrl = String.Empty;
        private readonly ICampaignService _campaignService;

        public CampaignsController(
            WhatsAppSolutionContext dbContext,
            ILogger<CampaignsController> logger,
          IOptions<BridgeConfigurationSettings> bridgeConfigurationSettings,
          IHttpClientFactory httpClientFactory,
          ICampaignService campaignService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _bridgeConfigurationSettings = bridgeConfigurationSettings;
            _httpClient = httpClientFactory.CreateClient(HttpClientType.bridge_api);
            baseUrl = _httpClient.BaseAddress.AbsoluteUri;
            _campaignService = campaignService;
        }

        [HttpGet("getcampaignlist")]
        public async Task<ActionResult> GetCampaignListAsync(int ClientId)
        {
            var res = await _campaignService.GetCampaignListAsync(ClientId);
            return Ok(new ApiResult()
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getcampaignbyid")]
        public ActionResult GetCampaignByIdAsync(int Id)
        {
            if (Id <= 0)
            {
                return NotFound("not found");
            }

            var response = _dbContext.Campaigns.Where(x => x.CampaignId == Id).FirstOrDefault();

            if (response == null)
            {
                return Ok(new ApiResult()
                {
                    Success = false,
                    Result = "",
                    Message = "No record found with this id"
                });
            }

            return Ok(new ApiResult()
            {
                Success = true,
                Result = response,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addcampaign")]
        public async Task<IActionResult> AddCampaignAsync([FromBody] CampaignDto campaign)
        {
            if (campaign == null)
            {
                return BadRequest();
            }

            var response = await _campaignService.AddCampaignAsync(campaign);
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

        [HttpPost("activatecampaign")]
        public async Task<IActionResult> ActivateCampaignAsync([FromBody] CampaignDto campaign)
        {
            if (campaign == null)
            {
                return BadRequest();
            }

            var response = await _campaignService.ActivateCampaignAsync(campaign);
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
        [HttpPut("updatecampaign")]
        public async Task<IActionResult> UpdateCampaignAsync([FromBody] CampaignDto campaign)
        {
            if (campaign == null)
            {
                return BadRequest();
            }

            var response = await _campaignService.UpdateCampaignAsync(campaign);
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
        [HttpPost("settlecampaign")]
        public async Task<IActionResult> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            if (ClientId <= 0 || CampaignId <= 0)
            {
                return NotFound("not found");
            }

            var response = await _campaignService.SettleCampaignAsync(ClientId, CampaignId);
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
