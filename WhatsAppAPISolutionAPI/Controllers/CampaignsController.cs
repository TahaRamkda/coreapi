using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Campaign;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CampaignsController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<CampaignsController> _logger;
        private readonly ICampaignService _campaignService;
        private readonly IUserService _userService;

        public CampaignsController(
            WhatsAppSolutionContext dbContext,
            ILogger<CampaignsController> logger,
            IHttpClientFactory httpClientFactory,
            ICampaignService campaignService,
            IUserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _campaignService = campaignService;
            _userService = userService;


            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpGet("getcampaignlist")]
        public async Task<ActionResult> GetCampaignListAsync(int CampaignId = 0, DateTime? FromDate = null, DateTime? ToDate = null, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue, int SenderId = 0)
        {
            _logger.LogInformation("Calling function GetCampaignListAsync request with ClientId={ClientId}, CampaignId={CampaignId}, FromDate={FromDate}, ToDate={ToDate}, SearchStr={SearchStr}, SortBy={SortBy}, PageNo={PageNo}, PageSize={PageSize}",
            clientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize);

            var res = await _campaignService.GetCampaignListAsync(clientId, CampaignId, FromDate, ToDate, SearchStr, SortBy, PageNo, PageSize, SenderId);

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
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please enter client id" });

            if (string.IsNullOrEmpty(campaign.CampaignName))
                return Ok(new ApiResult { Message = "Please enter campaign name" });

            if (campaign.TemplateId <= 0)
                return Ok(new ApiResult { Message = "Please select template" });

            var response = await _campaignService.AddCampaignAsync(clientId, userId, campaign);
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
                return BadRequest();

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (campaign.CampaignId <= 0)
                return Ok(new ApiResult { Message = "Please select campaign" });

            if (!campaign.ScheduleDate.HasValue)
                return Ok(new ApiResult { Message = "Please select schedule date" });

            var response = await _campaignService.ActivateCampaignAsync(clientId, userId, campaign);

            _logger.LogInformation("Received api ActivateCampaignAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

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
            _logger.LogInformation("Calling api UpdateCampaignAsync with request={requst}", JsonConvert.SerializeObject(campaign));

            if (campaign == null)
                return Ok(new ApiResult { Message = "Campaign not found" });

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (campaign.CampaignId <= 0)
                return Ok(new ApiResult { Message = "Please select campaign" });

            var response = await _campaignService.UpdateCampaignAsync(clientId, userId, campaign);

            _logger.LogInformation("Received api UpdateCampaignAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        [HttpPost("settlecampaign")]
        public async Task<IActionResult> SettleCampaignAsync(int CampaignId)
        {
            _logger.LogInformation("Calling api SettleCampaignAsync with ClientId={ClientId}, CampaignId={CampaignId}", clientId, CampaignId);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (CampaignId <= 0)
                return Ok(new ApiResult { Message = "Please select campaign" });

            var response = await _campaignService.SettleCampaignAsync(clientId, CampaignId);

            _logger.LogInformation("Received api SettleCampaignAsync response with data={data}", JsonConvert.SerializeObject(response));

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

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
            _logger.LogInformation("Calling api SendCampaignAsync with request={requst}", JsonConvert.SerializeObject(campaign));

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Please select client" });

            if (campaign.CampaignId <= 0)
                return Ok(new ApiResult { Message = "Campaign is required" });

            if (campaign.PhoneNumbers == null || !campaign.PhoneNumbers.Any())
                return Ok(new ApiResult { Message = "Please add atleast one phone number" });

            var response = await _campaignService.SendCampaignMessagesAsync(clientId, campaign);

            _logger.LogInformation("Received api SendCampaignAsync response with data={data}", JsonConvert.SerializeObject(response));

            return Ok(response);
        }

        [HttpGet("getcampaigncontactstats")]
        public async Task<ActionResult> GetCampaignContactStatsAsync(int CampaignId = 0)
        {
            _logger.LogInformation("Calling function GetCampaignListAsync request with ClientId={ClientId}, CampaignId={CampaignId}", clientId, CampaignId);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Client is required" });

            if (CampaignId <= 0)
                return Ok(new ApiResult { Message = "Campaign is required" });

            var res = await _campaignService.GetCampaignContactStatsAsync(clientId, CampaignId);

            _logger.LogInformation("Received GetCampaignListAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "No data found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("deletefrequentlycontactedcontacts")]
        public async Task<ActionResult> DeleteFrequentlyContactedContactsAsync(int CampaignId, int LastContactedInDays)
        {
            _logger.LogInformation("Calling function DeleteFrequentlyContactedContactsAsync request with ClientId={ClientId}, CampaignId={CampaignId},LastContactedInDays={LastContactedInDays}", clientId, CampaignId, LastContactedInDays);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Client is required" });

            if (CampaignId <= 0)
                return Ok(new ApiResult { Message = "Campaign is required" });

            if (LastContactedInDays <= 0)
                return Ok(new ApiResult { Message = "Last contacted in days is required" });

            var res = await _campaignService.DeleteFreqContactedContactsAsync(clientId, CampaignId, LastContactedInDays);

            _logger.LogInformation("Received DeleteFrequentlyContactedContactsAsync response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "No data found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getcampaigndetail")]
        public async Task<ActionResult> GetCampaignDetailAsync(int CampaignId)
        {
            _logger.LogInformation("Calling function GetCampaignDetailAsyn request with ClientId={ClientId}, CampaignId={CampaignId}", clientId, CampaignId);

            if (clientId <= 0)
                return Ok(new ApiResult { Message = "Client is required" });

            if (CampaignId <= 0)
                return Ok(new ApiResult { Message = "Campaign is required" });

            var res = await _campaignService.GetCampaignDetailAsync(clientId, CampaignId);

            _logger.LogInformation("Received GetCampaignDetailAsyn response with data={data}", JsonConvert.SerializeObject(res));

            if (res == null)
                return Ok(new ApiResult { Message = "No data found" });

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
