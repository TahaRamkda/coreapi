using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IGroupService
    {
        Task<List<UGroup>> GetGroupListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponse> AddGroupAsync(GroupDto group);
        Task<UResponse> UpdateGroupAsync(GroupDto group);
        Task<UResponse> DeleteGroupAsync(int groupId);
        Task<List<UEntityDto>> GetGroupsAsync(int clientId, string searchStr = "");
    }
}
