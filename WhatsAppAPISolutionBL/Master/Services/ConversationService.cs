using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ConversationService : IConversationService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ConversationService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UConversation>> GetConversationListAsync(int clientId = 0, int id = 0, string conversationId = "", int senderId = 0,
            string waId = "", int moduleId = 0, int parentId = 0,
            int agentId = 0, int status = 0, string phoneNumber = "",
            string searchStr = "", int sortBy = 0, int pageNo = 0, int pageSize = int.MaxValue)
        {
            var response = await _dbContext2.Conversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.List},@ClientId={clientId},@Id={id},@ConversationId={conversationId},@SenderId={senderId},@WaId={waId},@ModuleId={moduleId},@ParentId={parentId},@AgentId={agentId}, @Status={status}, @PhoneNumber={phoneNumber}, @SearchStr={searchStr},@SortBy={sortBy}, @PageNo={pageNo}, @PageSize={pageSize}").ToListAsync();
            return response;
        }

        public async Task<List<UAgentConversationList>> GetAgentConversationListAsync(int clientId = 0, int id = 0, int senderId = 0, int agentId = 0)
        {
            var response = await _dbContext2.AgentConversationLists.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.AgentConversationList},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}").ToListAsync();
            return response;
        }

        public async Task<List<UConversationListByConversation>> GetConversationListByConversationAsync(int clientId = 0, int id = 0, int senderId = 0, int agentId = 0)
        {
            var response = await _dbContext2.ConversationListByConversations.FromSqlInterpolated($"exec usp_Conversations_Ops @ActionId={(int)CrudEnum.ConversationListByConversation},@ClientId={clientId},@Id={id},@SenderId={senderId},@AgentId={agentId}").ToListAsync();
            return response;
        }
    }
}
