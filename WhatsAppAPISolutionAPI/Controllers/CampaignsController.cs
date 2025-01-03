using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WhatsAppAPISolutionAPI.Setting;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Campaign;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignsController : ControllerBase
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CampaignsController> _logger;
        private readonly ICampaignService _campaignService;

        public CampaignsController(
            WhatsAppSolutionContext dbContext,
            ILogger<CampaignsController> logger,
            IHttpClientFactory httpClientFactory,
            ICampaignService campaignService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _campaignService = campaignService;
        }

        [HttpGet("getcampaignlist")]
        public async Task<ActionResult> GetCampaignListAsync(int ClientId, int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            _logger.LogInformation("Calling function GetCampaignListAsync request with ClientId={ClientId}, CampaignId={CampaignId}, FromDate={FromDate}, ToDate={ToDate}, SearchStr={SearchStr}, SortBy={SortBy}, PageNo={PageNo}, PageSize={PageSize}",
            ClientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize);

            var res = await _campaignService.GetCampaignListAsync(ClientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize, SenderId);

            _logger.LogInformation("Received GetCampaignListAsync response with data={data}", JsonConvert.SerializeObject(res));

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("addcampaign")]
        public async Task<IActionResult> AddCampaignAsync([FromBody] CampaignDto campaign)
        {
            _logger.LogInformation("Calling function AddCampaignAsync with request = {request}", JsonConvert.SerializeObject(campaign));

            if (campaign == null)
            {
                return BadRequest();
            }

            if (campaign.TemplateId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select template"
                });
            }

            var response = await _campaignService.AddCampaignAsync(campaign);
            if (response == null || response.Status <= 0)
            {
                _logger.LogError("Received response from AddCampaignAsync with error = {error}", JsonConvert.SerializeObject(response?.Message));

                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

            _logger.LogInformation("Received response from AddCampaignAsync with response = {response}", JsonConvert.SerializeObject(response));

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("activatecampaign")]
        public async Task<IActionResult> ActivateCampaignAsync([FromBody] ActivateCampaignDto campaign)
        {
            _logger.LogInformation("Calling function ActivateCampaignAsync with request = {request}", JsonConvert.SerializeObject(campaign));

            if (campaign == null)
            {
                return BadRequest();
            }

            if (campaign.ClientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select client"
                });
            }

            if (campaign.CampaignId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select campaign"
                });
            }

            if (!campaign.ScheduleDate.HasValue)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select schedule date"
                });
            }

            var response = await _campaignService.ActivateCampaignAsync(campaign);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

            return Ok(new ApiResult
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

            if (campaign.ClientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select client"
                });
            }

            if (campaign.CampaignId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select campaign"
                });
            }

            var response = await _campaignService.UpdateCampaignAsync(campaign);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("settlecampaign")]
        public async Task<IActionResult> SettleCampaignAsync(int ClientId, int CampaignId)
        {
            if (ClientId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select client"
                });
            }

            if (CampaignId <= 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Please select campaign"
                });
            }

            var response = await _campaignService.SettleCampaignAsync(ClientId, CampaignId);
            if (response == null || response.Status <= 0)
            {
                return Ok(new ApiResult
                {
                    Result = response,
                    Message = response?.Message
                });
            }
            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("sendcampaign")]
        public async Task<IActionResult> SendCampaignAsync([FromBody] SendCampaignDto campaign)
        {
            if (campaign.CampaignId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Campaign is required"
                });
            if (campaign.PhoneNumbers == null || !campaign.PhoneNumbers.Any())
                return Ok(new ApiResult
                {
                    Message = "Please add atleast one phone number"
                });

            var response = await _campaignService.SendCampaignMessagesAsync(campaign);
            return Ok(response);
        }

        [HttpGet("getcampaigncontactstats")]
        public async Task<ActionResult> GetCampaignContactStatsAsync(int ClientId, int CampaignId = 0)
        {
            _logger.LogInformation("Calling function GetCampaignListAsync request with ClientId={ClientId}, CampaignId={CampaignId}", ClientId, CampaignId);

            if (ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Client is required"
                });

            if (CampaignId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Campaign is required"
                });

            var res = await _campaignService.GetCampaignContactStatsAsync(ClientId, CampaignId);

            _logger.LogInformation("Received GetCampaignListAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No data found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("deletefrequentlycontactedcontacts")]
        public async Task<ActionResult> DeleteFrequentlyContactedContactsAsync(int ClientId, int CampaignId, int LastContactedInDays)
        {
            _logger.LogInformation("Calling function DeleteFrequentlyContactedContactsAsync request with ClientId={ClientId}, CampaignId={CampaignId},LastContactedInDays={LastContactedInDays}", ClientId, CampaignId, LastContactedInDays);

            if (ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Client is required"
                });

            if (CampaignId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Campaign is required"
                });

            if (LastContactedInDays <= 0)
                return Ok(new ApiResult
                {
                    Message = "Last contacted in days is required"
                });

            var res = await _campaignService.DeleteFreqContactedContactsAsync(ClientId, CampaignId, LastContactedInDays);

            _logger.LogInformation("Received DeleteFrequentlyContactedContactsAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No data found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getcampaigndetail")]
        public async Task<ActionResult> GetCampaignDetailAsync(int ClientId, int CampaignId)
        {
            _logger.LogInformation("Calling function GetCampaignDetailAsyn request with ClientId={ClientId}, CampaignId={CampaignId}", ClientId, CampaignId);

            if (ClientId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Client is required"
                });

            if (CampaignId <= 0)
                return Ok(new ApiResult
                {
                    Message = "Campaign is required"
                });

            var res = await _campaignService.GetCampaignDetailAsync(ClientId, CampaignId);

            _logger.LogInformation("Received GetCampaignDetailAsyn response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
            {
                return Ok(new ApiResult
                {
                    Message = "No data found"
                });
            }

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
