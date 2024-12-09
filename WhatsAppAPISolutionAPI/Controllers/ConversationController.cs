using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionAPI.Models;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionBL.Master.Services;

namespace WhatsAppAPISolutionAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly ILogger<ConversationController> _logger;
        private readonly IConversationService _conversationService;

        public ConversationController(ILogger<ConversationController> logger,
            IConversationService conversationService)
        {
            _logger = logger;
            _conversationService = conversationService;
        }

        [HttpGet("getconversationlist")]
        public async Task<ActionResult> GetConversationListAsync(int clientId = 0, int id = 0, string conversationId = "", int senderId = 0,
           string waId = "", int moduleId = 0, int parentId = 0,
           int agentId = 0, int status = 0, string phoneNumber = "",
           string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _conversationService.GetConversationListAsync(clientId, id, conversationId, senderId,
                waId, moduleId, parentId,
                agentId, status, phoneNumber,
                searchStr, sortBy, pageNo, pageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getagentconversationlist")]
        public async Task<ActionResult> GetAgentConversationListAsync(int clientId = 0, int id = 0, int senderId = 0, int agentId = 0)
        {
            var res = await _conversationService.GetAgentConversationListAsync(clientId, id, senderId, agentId);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getconversationlistbyid")]
        public async Task<ActionResult> GetConversationListByIdAsync(int clientId = 0, int id = 0, int senderId = 0, int agentId = 0)
        {
            var res = await _conversationService.GetConversationListByConversationAsync(clientId, id, senderId, agentId);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }
    }
}
