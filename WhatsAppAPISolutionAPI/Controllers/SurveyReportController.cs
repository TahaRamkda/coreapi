using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SurveyReportController : ControllerBase
    {
        private readonly int clientId;
        private readonly ISurveyReportService _surveyReportService;
        private readonly IUserService _userService;
        
        public SurveyReportController(ISurveyReportService surveyReportService, IUserService userService)
        {
            _surveyReportService = surveyReportService;
            _userService = userService;

            clientId = _userService.GetClientIdFromAccessToken();
        }

        [HttpGet("getsurveyreport")]
        public async Task<ActionResult> GetSurveyReport(int? senderId, int? flowId, string phoneNumber, string searchText)
        {
            var result = await _surveyReportService.GetSurveyReportAsync(clientId, senderId, flowId, phoneNumber, searchText);
            return Ok(new ApiResult
            {
                Success = true,
                Message = "Survey report fetched successfully",
                Result = result
            });
        }
    }
}
