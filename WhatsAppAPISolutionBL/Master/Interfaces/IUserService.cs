using WhatsAppAPISolutionDL.Dto.Client;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.User;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IUserService
    {
        int GetUserIdFromAccessToken();
        int GetClientIdFromAccessToken();
        Task<List<User>> GetUserListAsync(int ClientId); 
        Task<UUser> Login(string UserName, string Password, int MasterRoleTypeId = 0);
        Task<UResponse> AddUserAsync(UserDto user);
        Task<UResponse> UpdateUserAsync(UserDto user);
        Task<UResponse> AddUserTokenAsync(UserDto user);
        Task<UResponse> DeleteUserAsync(int UserId, int ClientId);
        Task<UResponse> ChangePasswordAsync(UserDto user);
        Task<UResponse> ResetPasswordAsync(ResetPassword password);
        Task<UUserDetail> GetUserByIdAsync(int clientId, int userId);
        Task<UResponse> OnboardNewClient(OnboardClientDto requestDto);
        Task<List<UUserList>> GetUsersListAsync(int clientId, string searchStr = "");
    }
}
