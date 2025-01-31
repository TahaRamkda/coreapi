using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Common;
using WhatsAppAPISolutionDL.Dto.Conversation;

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
            _logger.LogInformation("Calling api GetConversationListAsync with clientId={clientId}, senderId={senderId}, id={id}, conversationId={conversationId}, waId={waId}, moduleId={moduleId}, parentId={parentId}, agentId={agentId}, status={status}, phoneNumber={phoneNumber}, searchStr={searchStr}, sortBy={sortBy}, pageNo={pageNo}, pageSize={pageSize}", clientId, senderId, id, conversationId, waId, moduleId, parentId, agentId, status, phoneNumber, searchStr, sortBy, pageNo, pageSize);

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
            _logger.LogInformation("Calling api GetAgentConversationListAsync with clientId={clientId}, senderId={senderId}, id={id}, agentId={agentId}, pageNo={pageNo}, pageSize={pageSize}", clientId, senderId, id, agentId, pageNo, pageSize);

            var res = await _conversationService.GetAgentConversationListAsync(clientId, senderId, id, agentId, pageNo, pageSize);

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
            _logger.LogInformation("Calling api GetConversationMessageByIdAsync with clientId={clientId}, senderId={senderId}, id={id}, agentId={agentId}, messageId={messageId}, pageNo={pageNo}, pageSize={pageSize}", clientId, senderId, id, agentId, messageId, pageNo, pageSize);

            var res = await _conversationService.GetConversationListByConversationAsync(clientId, senderId, id, agentId, messageId, pageNo, pageSize);

            _logger.LogInformation("Received api GetConversationMessageByIdAsync response with data={data}", JsonConvert.SerializeObject(res));

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
            _logger.LogInformation("Calling api AddConversationToQueueAsync with clientId={clientId}, id={id}, comment={comment}", clientId, id, comment);

            var response = await _conversationService.AddConversationToQueueAsync(clientId, id, comment);

            _logger.LogInformation("Received api AddConversationToQueueAsync response with data={data}", JsonConvert.SerializeObject(response));

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
        public async Task<ActionResult> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int oldAgentId = 0, int agentId = 0, string comment = "")
        {
            _logger.LogInformation("Calling api TransferConversationToAgentAsync with clientId={clientId}, id={id}, oldAgentId={oldAgentId}, agentId={agentId}, comment={comment}", clientId, id, oldAgentId, agentId, comment);

            var response = await _conversationService.TransferConversationToAgentAsync(clientId, id, oldAgentId, agentId, comment);

            _logger.LogInformation("Received api TransferConversationToAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [AllowAnonymous]
        [HttpPost("assignconversationtoagent")]
        public async Task<ActionResult> AssignConversationToAgentAsync(List<AssignConversationDto> models)
        {
            _logger.LogInformation("Calling function AssignConversationToAgentAsync with data={data}", JsonConvert.SerializeObject(models));

            if (models == null || models.Count == 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Invalid data provided"
                });
            }

            var response = await _conversationService.AssignConversationToAgentAsync(models);

            _logger.LogInformation("Received api AssignConversationToAgentAsync response with data={data}", JsonConvert.SerializeObject(response));

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

        [HttpGet("getconversationreportlist")]
        public async Task<ActionResult> GetConversationReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", string searchStr = "")
        {
            _logger.LogInformation("Calling api GetConversationReportListAsync with clientId={clientId}, senderId={senderId}, id={id}, agentId={agentId}, pageNo={pageNo}, pageSize={pageSize}, status={status}, searchStr={searchStr}", clientId, senderId, id, agentId, pageNo, pageSize, status, searchStr);

            var res = await _conversationService.GetConversationReportListAsync(clientId, senderId, id, agentId, pageNo, pageSize, status, searchStr);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpGet("getconversationdetailreportlist")]
        public async Task<ActionResult> GetConversationDetailReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "")
        {
            _logger.LogInformation("Calling api GetConversationDetailReportListAsync with clientId={clientId}, senderId={senderId}, id={id}, agentId={agentId}, pageNo={pageNo}, pageSize={pageSize}, status={status}, fromDate={fromDate}, toDate={toDate}, searchStr={searchStr}", clientId, senderId, id, agentId, pageNo, pageSize, status, fromDate, toDate, searchStr);

            var res = await _conversationService.GetConversationDetailReportListAsync(clientId, senderId, id, agentId, pageNo, pageSize, status, fromDate, toDate, searchStr);

            return Ok(new ApiResult
            {
                Success = true,
                Result = res,
                Message = "Data fetch successfully"
            });
        }

        [HttpPost("expiredconversationnotify")]
        public async Task<ActionResult> ExpiredConversationNotifyAsync(List<ExpiredConversationDto> models)
        {
            _logger.LogInformation("Calling function ExpiredConversationNotifyAsync with data={data}", JsonConvert.SerializeObject(models));

            if (models == null || models.Count == 0)
            {
                return Ok(new ApiResult
                {
                    Message = "Invalid data provided"
                });
            }

            var response = await _conversationService.ExpiredConversationNotifyToAgentAsync(models);

            _logger.LogInformation("Received api ExpiredConversationNotifyAsync response with data={data}", JsonConvert.SerializeObject(response));

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
