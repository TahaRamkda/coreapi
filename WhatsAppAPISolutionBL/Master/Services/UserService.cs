using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.User;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class UserService : IUserService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public UserService(WhatsAppSolutionContext2 dbContext2,
            WhatsAppSolutionContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetClientIdFromAccessToken()
        {
            int id = 0;
            if (User != null)
                int.TryParse(User?.FindFirst("ClientId")?.Value, out id);
            return id;
        }

        public int GetUserIdFromAccessToken()
        {
            int id = 0;
            if (User != null)
                int.TryParse(User?.FindFirst("UserId")?.Value, out id);
            return id;
        }
         
        public Task<List<User>> GetUserListAsync(int ClientId)
        {
            throw new NotImplementedException();
        }

        public async Task<UUser> Login(string UserName, string Password, int MasterRoleTypeId = 0)
        {
            var userDetails = await _dbContext2.Users.FromSqlInterpolated($"exec usp_Users_Login @UserName={UserName}, @Password={Password}, @MasterRoleTypeId={MasterRoleTypeId}").IgnoreQueryFilters().ToListAsync();
            if (userDetails != null && userDetails[0].Status > 0)
                userDetails[0].Permission = await _dbContext2.Permissions.FromSqlInterpolated($"exec usp_GetUserPermission @UserId={userDetails[0].UserId}").IgnoreQueryFilters().ToListAsync();

            return userDetails[0];
        }

        public Task<User> RegisterUser(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<UResponse> AddUserAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={user.ClientId}, @UserName={user.UserName}, @Password={user.Password}, @IsActive={user.IsActive}, @FullName={user.FullName}, @ActionBy={user.ActionBy}, @UserRoles={user.UserRoles}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateUserAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={user.ClientId}, @UserId={user.UserId}, @UserName={user.UserName}, @IsActive={user.IsActive}, @FullName={user.FullName}, @ActionBy={user.ActionBy}, @UserRoles={user.UserRoles}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteUserAsync(int UserId, int ClientId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Delete}, @UserId={UserId}, @ClientId={ClientId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> ChangePasswordAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.ChangePassword}, @ClientId={user.ClientId}, @UserId={user.UserId}, @UserName={user.UserName}, @Password={user.Password}, @OldPassword={user.OldPassword}").ToListAsync();
            return response[0];
        }

        public async Task<List<UUserList>> GetUsersListAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.UserLists.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddUserTokenAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.AddUserToken}, @ClientId={user.ClientId}, @UserId={user.UserId}, @AccessToken={user.AccessToken}, @RefreshToken={user.RefreshToken}, @RefreshTokenExpiry={user.RefreshTokenExpiry}").ToListAsync();
            return response[0];
        }

        public async Task<UUserDetail> GetUserByIdAsync(int clientId, int userId)
        {
            var response = await _dbContext2.UserDetails.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}, @UserId={userId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
