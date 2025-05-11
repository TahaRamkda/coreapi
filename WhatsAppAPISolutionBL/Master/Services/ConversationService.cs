using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Conversation;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Hubs;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ConversationService : IConversationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<ConversationService> _logger;
        private readonly IHubContext<ConversationHub> _conversationHubContext; 
        private readonly IMediatorService _mediatorService;

        public ConversationService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<ConversationService> logger,
            IHubContext<ConversationHub> conversationHubContext,
            IMediatorService mediatorService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _conversationHubContext = conversationHubContext;
            _mediatorService = mediatorService;
        }

        public async Task<List<UConversation>> GetConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, string conversationId = "",
            string waId = "", int moduleId = 0, int parentId = 0,
            int agentId = 0, int status = 0, string phoneNumber = "",
            string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Conversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.List},@ClientId={clientId},@Id={id},@ConversationId={conversationId},@SenderId={senderId},@WaId={waId},@ModuleId={moduleId},@ParentId={parentId},@AgentId={agentId}, @Status={status}, @PhoneNumber={phoneNumber}, @SearchStr={searchStr},@SortBy={sortBy}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();

            _logger.LogDebug("Calling procedure usp_Conversations_Ops with actionId = {actionId}, actionName = {actionName} and ProcResponseTime={ProcResponseTime} ", (int)CrudEnum.List, CrudEnum.List, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);

            return response;
        }

        public async Task<List<UAgentConversationList>> GetAgentConversationListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.AgentConversationLists.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AgentConversationList},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with parameters: ActionId={ActionId}, ActionName={ActionName}, ClientId={ClientId}, SenderId={SenderId}, Id={Id}, AgentId={AgentId}, PageNo={PageNo}, PageSize={PageSize}, ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.AgentConversationList, CrudEnum.AgentConversationList, clientId, senderId, id, agentId, pageNo, pageSize, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<List<UConversationListByConversation>> GetConversationListByConversationAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int messageId = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ConversationListByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationListByConversation},@ClientId={clientId},@Id={id}, @messageId={messageId},@SenderId={senderId},@AgentId={agentId}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with parameters: ActionId={ActionId}, ActionName={ActionName}, ClientId={ClientId}, SenderId={SenderId}, Id={Id}, AgentId={AgentId}, PageNo={PageNo}, PageSize={PageSize},messageId={messageId}, ProcResponseTime={ProcResponseTime}ms", (int)CrudEnum.AgentConversationList, CrudEnum.AgentConversationList, clientId, senderId, id, agentId, pageNo, pageSize, messageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<UResponse> AddConversationToQueueAsync(int clientId = 0, int id = 0, string comment = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AddConversationToQueue},@ClientId={clientId},@Id={id}, @Comment={comment}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, id={id}, , comment = {comment}, actionId = {actionId}, actionName = {actionName} and ProcResponseTime={ProcResponseTime} ", clientId, id, comment, (int)CrudEnum.AddConversationToQueue, CrudEnum.AddConversationToQueue, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            if (response == null || !response.Any())
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot add conversation to queue"
                };
            }
            return response[0];

        }


        public async Task<UResponse> TransferConversationToAgentAsync(int clientId = 0, int id = 0, int oldAgentId = 0, int agentId = 0, string comment = "")
        {
            var response = await _mediatorService.TransferConversationToAgentAsync(clientId, id, oldAgentId, agentId, comment);
            return response;
        }

        /// <summary>
        /// This call will be call by assign agent background service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UResponse> AssignConversationToAgentAsync(List<AssignConversationDto> models)
        {
            await _mediatorService.AssignConversationToAgentAsync(models);
 
            return new UResponse
            {
                Status = 1,
                Message = "Data updated successfully"
            };
        }

        //public async Task<ULatestConversationByConversation> GetLatestConversationMessageByConversationAsync(int clientId = 0, int id = 0)
        //{
        //    var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetLatestConversationByConversationId},@ClientId={clientId},@Id={id}").ToListAsync();

        //    if (response.Any())
        //        return response[0];

        //    return null;
        //}

        public async Task<ULatestConversationByConversation> GetConversationMessageByMessageIdAsync(int clientId = 0, int senderId = 0, int conversationId = 0, int conversationMessageId = 0, int status = 0)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.LatestConversationByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationByMessageId},@ClientId={clientId}, @SenderId={senderId},@Id={conversationId}, @MessageId={conversationMessageId}, @Status={status}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={clientId}, senderId={senderId}, conversationId={conversationId}, conversationMessageId={conversationMessageId}, status={status}, actionId={actionId}, actionName={actionName} and ProcResponseTime={ProcResponseTime} ", clientId, senderId, conversationId, conversationMessageId, status, (int)CrudEnum.GetConversationByMessageId, CrudEnum.GetConversationByMessageId, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            if (response.Any())
                return response[0];

            return null;
        }

        public async Task<List<UConversationReportList>> GetConversationReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", string searchStr = "", string fChatInitiated = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ConversationReports.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationReportList},@ClientId={clientId},@Id={id},@SenderId={senderId}, @AgentId={agentId}, @FStatus={status}, @PageNo={pageNo}, @PageSize={pageSize}, @SearchStr={searchStr}, @FChatInitiated={fChatInitiated}").ToListAsync();
            _logger.LogInformation(
                "Calling procedure usp_Conversations_Ops with clientId={ClientId}, senderId={SenderId}, id={Id}, agentId={AgentId}, pageNo={PageNo}, pageSize={PageSize}, status={Status}, searchStr={SearchStr}, fChatInitiated={FChatInitiated}, actionId={ActionId}, actionName={ActionName}, ProcResponseTime={ProcResponseTime}ms", clientId, senderId, id, agentId, pageNo,
                pageSize, status, searchStr, fChatInitiated, (int)CrudEnum.ConversationReportList,
                CrudEnum.ConversationReportList,
                DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<List<UConversationReportList>> GetConversationDetailReportListAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "", string fChatInitiated = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ConversationReports.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationDetailReportList},@ClientId={clientId},@Id={id},@SenderId={senderId}, @AgentId={agentId}, @FStatus={status}, @PageNo={pageNo}, @PageSize={pageSize}, @FromDate={fromDate}, @ToDate={toDate}, @SearchStr={searchStr}, @FChatInitiated={fChatInitiated}").ToListAsync();
            _logger.LogInformation("Calling procedure usp_Conversations_Ops with clientId={ClientId}, senderId={SenderId}, id={Id}, agentId={AgentId}, status={Status}, pageNo={PageNo}, pageSize={PageSize}, fromDate={FromDate}, toDate={ToDate}, searchStr={SearchStr}, fChatInitiated={FChatInitiated}, actionId={ActionId}, actionName={ActionName}, ProcResponseTime={ProcResponseTime}ms",
                clientId, senderId, id, agentId, status, pageNo, pageSize, fromDate, toDate, searchStr, fChatInitiated, (int)CrudEnum.ConversationDetailReportList, CrudEnum.ConversationDetailReportList, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<UResponse> ExpiredConversationNotifyToAgentAsync(List<ExpiredConversationDto> models)
        {
            var response = await _mediatorService.ExpiredConversationNotifyToAgentAsync(models);
            return response;
        }

        public async Task<UResponse> CloseChatBySupervisor(int id)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.CloseChatBySupervisor}, @Id={id}").ToListAsync();
            _logger.LogDebug("Calling procedure usp_Conversations_Ops with actionId = {actionId}, actionName = {actionName} and ProcResponseTime={ProcResponseTime} ", (int)CrudEnum.CloseChatBySupervisor, CrudEnum.CloseChatBySupervisor, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response[0];
        }

        public async Task<List<UConversationLogsList>> GetConversationLogsListAsync(int clientId = 0, int conversationId = 0)
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ConversationLogsList.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationLogs},@ClientId={clientId}, @Id={conversationId}").ToListAsync();
            _logger.LogInformation(
                "Calling procedure usp_Conversations_Ops with clientId={ClientId}, conversationId={ConversationId}, actionId={ActionId}, actionName={ActionName}, ProcResponseTime={ProcResponseTime}ms",
                clientId, conversationId, (int)CrudEnum.GetConversationLogs, CrudEnum.GetConversationLogs, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds);
            return response;
        }

        public async Task<UConversationStatistics> GetConversationStatisticsAsync(int clientId = 0, int senderId = 0, int id = 0, int agentId = 0, int pageNo = 0, int pageSize = int.MaxValue, string status = "", DateTime? fromDate = null, DateTime? toDate = null, string searchStr = "", string fChatInitiated = "")
        {
            var startProcTime = DateTime.UtcNow;
            var response = await _dbContext2.ConversationStatistics.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.GetConversationStatistics},@ClientId={clientId},@Id={id},@SenderId={senderId}, @AgentId={agentId}, @FStatus={status}, @PageNo={pageNo}, @PageSize={pageSize}, @FromDate={fromDate}, @ToDate={toDate}, @SearchStr={searchStr}, @FChatInitiated={fChatInitiated}").ToListAsync();
            _logger.LogDebug(
                "Calling procedure usp_Conversations_Ops with clientId={ClientId}, senderId={SenderId}, id={Id}, agentId={AgentId}, status={Status}, pageNo={PageNo}, pageSize={PageSize}, fromDate={FromDate}, toDate={ToDate}, searchStr={SearchStr}, fChatInitiated={FChatInitiated}, actionId={ActionId}, actionName={ActionName}, ProcResponseTime={ProcResponseTime}ms",
                clientId, senderId, id, agentId, status, pageNo, pageSize, fromDate, toDate, searchStr, fChatInitiated, (int)CrudEnum.GetConversationStatistics, CrudEnum.GetConversationStatistics, DateTime.UtcNow.Subtract(startProcTime).TotalMilliseconds
            ); return response[0];
        }
    }
}
