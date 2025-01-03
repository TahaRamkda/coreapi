using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Group;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Group;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class GroupService : IGroupService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public GroupService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UGroup>> GetGroupListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Groups.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddGroupAsync(GroupDto group)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={group.ClientId}, @GroupName={group.GroupName}, @ActionBy={group.ActionBy}").ToListAsync();
            return response[0];
        }


        public async Task<UResponse> UpdateGroupAsync(GroupDto group)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={group.ClientId}, @GroupId={group.GroupId}, @GroupName={group.GroupName}, @ActionBy={group.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteGroupAsync(int GroupId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.Delete}, @GroupId={GroupId}").ToListAsync();
            return response[0];
        }

        public async Task<List<UEntityDto>> GetGroupsAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<UGroupDetail> GetGroupByIdAsync(int clientId, int groupId)
        {
            var response = await _dbContext2.GroupDetails.FromSqlInterpolated($"exec usp_Groups_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId},@GroupId={groupId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        } 
    }
}
