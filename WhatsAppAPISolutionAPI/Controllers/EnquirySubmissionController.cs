using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.ConversationAnalytic;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class EnquirySubmissionController : ControllerBase
    {
        private readonly IEnquirySubmission _enquirySubmission;
        private readonly ILogger<EnquirySubmissionController> _logger;
        private readonly int clientId;
        private readonly IUserService _userService;

        public EnquirySubmissionController(IEnquirySubmission enquirySubmission, ILogger<EnquirySubmissionController> logger, IUserService userService )
        {
            _enquirySubmission = enquirySubmission;
            _logger = logger;
            _userService = userService;
            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("GetEnquiryList")]
        public async Task<IActionResult> GetEnquirySubmissionList(int senderId, int enquiryId,  int sortBy, DateTime? FromDate, DateTime? ToDate, int pageNo=0, int pageSize=1000)
        {
            _logger.LogInformation("GetEnquirySubmissionList called");
            var result = await _enquirySubmission.GetEnquirySubmissionListsAsync(clientId, senderId,enquiryId, sortBy, FromDate, ToDate, pageNo, pageSize);
            return Ok(result);
        }
        [HttpGet("GetEnquiryDetails")]
        public async Task<IActionResult> GetEnquiryDetails(int senderId, string SearchStr="")
        {
            _logger.LogInformation("GetEnquirySubmissionList called");
            var result = await _enquirySubmission.GetUEnquirySubmissionDetails(clientId, senderId, SearchStr);
            return Ok(result);
        }
    }
}
