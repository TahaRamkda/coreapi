using WhatsAppAPISolutionDL.Dto.Group;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.Group;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IGroupService
    {
        Task<List<UGroup>> GetGroupListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponse> AddGroupAsync(int clientId, int userId, GroupDto group);
        Task<UResponse> UpdateGroupAsync(int clientId, int userId, GroupDto group);
        Task<UResponse> DeleteGroupAsync(int groupId);
        Task<List<UEntityDto>> GetGroupsAsync(int clientId, string searchStr = "");
        Task<UGroupDetail> GetGroupByIdAsync(int clientId, int groupId);
    }
}
