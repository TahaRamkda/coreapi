using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.User;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IRoleService
    {
        Task<List<URole>> GetRoleListAsync(int ClientId);
        Task<UResponse> AddRoleAsync(RoleDto role);
        Task<UResponse> UpdateRoleAsync(RoleDto role);
        Task<UResponse> DeleteRoleAsync(int RoleId);
        Task<List<UEntityDto>> GetRolesAsync(int clientId, string searchStr = "");
        Task<URoleDetail> GetRoleByIdAsync(int clientId, int roleId);
    }
}
