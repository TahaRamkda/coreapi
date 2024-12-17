using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;

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
        public async Task<ActionResult> GetConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, string conversationId = "",
           string waId = "", int moduleId = 0, int parentId = 0,
           int agentId = 0, int status = 0, string phoneNumber = "",
           string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _conversationService.GetConversationListAsync(clientId, senderId, id, conversationId,
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
        public async Task<ActionResult> GetAgentConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _conversationService.GetAgentConversationListAsync(clientId, senderId, id, agentId);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getconversationmessagebyid")]
        public async Task<ActionResult> GetConversationMessageByIdAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int messageId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var res = await _conversationService.GetConversationListByConversationAsync(clientId, senderId, id, agentId, messageId, pageNo, pageSize);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("addconversationtoqueue")]
        public async Task<ActionResult> AddConversationToQueueAsync(int clientId = 0, int id = 0, string comment = "")
        {
            var response = await _conversationService.AddConversationToQueueAsync(clientId, id, comment);

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
                Message = "Data updated successfully"
            });
        }

        [HttpGet("transferconversationtoagent")]
        public async Task<ActionResult> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int agentId = 0, string comment = "")
        {
            var response = await _conversationService.TransferConversationToAgentAsync(clientId, id, agentId, comment);

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
                Message = "Data updated successfully"
            });
        }

        [HttpGet("assignconversationtoagent")]
        public async Task<ActionResult> AssignConversationToAgentAsync(int clientId = 0, int id = 0, int agentId = 0, string comment = "")
        {
            var response = await _conversationService.AssignConversationToAgentAsync(clientId, id, agentId, comment);

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
                Message = "Data updated successfully"
            });
        }
    }
}
