using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.DTO.Survey;
using Azure;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.Dto.Flows;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class FlowsController : ControllerBase
    {
        private readonly int clientId;
        private readonly int userId;
        //private readonly ISurveyService _surveyService;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly ILogger<FlowsController> _logger;
        private readonly IFlowsService _flowsService;
        private readonly IUserService _userService;

        public FlowsController(IFlowsService flowsService,
            WhatsAppSolutionContext dbContext,
            ILogger<FlowsController> logger,
            IUserService userService)
        {
            _flowsService = flowsService;
            _dbContext = dbContext;
            _logger = logger;
            _flowsService = flowsService;
            _userService = userService;

            clientId = _userService.GetClientIdFromAccessToken();
            userId = _userService.GetUserIdFromAccessToken();
        }

        [HttpPost]
        public async Task<ActionResult> CreateFlows(FlowDTO obj)
        {
            var response = await _flowsService.CreateFlows(clientId, userId, obj);

            if (response == null || response.Status <= 0)
                return Ok(new ApiResult { Message = response?.Message });

            return Ok(new ApiResult
            {
                Success = true,
                Result = response,
                Message = "Data added successfully"
            });
        }

        //[HttpPost]
        //public async Task<ActionResult> InsertFlowsResponse(InsertFlowDTO obj)
        //{
        //    var response = await _flowsService.CreateFlows(clientId, userId, obj);

        //    if (response == null || response.Status <= 0)
        //        return Ok(new ApiResult { Message = response?.Message });

        //    return Ok(new ApiResult
        //    {
        //        Success = true,
        //        Result = response,
        //        Message = "Data added successfully"
        //    });
        //}

    }
}