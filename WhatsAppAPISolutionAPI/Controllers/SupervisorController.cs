using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Models;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SupervisorController : ControllerBase
    {
        private readonly int clientId;
        private readonly IUserService _userService;
        private readonly ILogger<SupervisorController> _logger;
        private readonly ISupervisorService _supervisorService;

        public SupervisorController(IUserService userService,
            ILogger<SupervisorController> logger,
            ISupervisorService supervisorService)
        {
            _userService = userService;
            clientId = _userService.GetClientIdFromAccessToken();
            _logger = logger;
            _supervisorService = supervisorService;
        }

        [HttpGet("getsupervisordashboard")]
        public async Task<ActionResult> GetSupervisorDashboardAsync(int senderId = 0)
        {
            _logger.LogInformation("Calling api GetSupervisorDashboardAsync with clientId={clientId}, senderId={senderId}", clientId, senderId);

            var res = await _supervisorService.GetSupervisorDashboardAsync(clientId, senderId);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

    }
}
