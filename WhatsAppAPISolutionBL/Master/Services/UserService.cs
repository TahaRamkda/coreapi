using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WhatsAppAPISolutionBL.Helper;
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
        private readonly ILogger<UserService> _logger;
        private readonly ICacheService _cacheService;   
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public UserService(WhatsAppSolutionContext2 dbContext2,
            WhatsAppSolutionContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _httpContextAccessor = httpContextAccessor;
            _cacheService = cacheService;
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
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={user.ClientId}, @UserName={user.UserName}, @Password={user.Password}, @IsActive={user.IsActive}, @FullName={user.FullName}, @ActionBy={GetUserIdFromAccessToken()}, @UserRoles={user.UserRoles}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateUserAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={user.ClientId}, @UserId={user.UserId}, @UserName={user.UserName}, @IsActive={user.IsActive}, @FullName={user.FullName}, @ActionBy={GetUserIdFromAccessToken()}, @UserRoles={user.UserRoles}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteUserAsync(int UserId, int ClientId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.Delete}, @UserId={UserId}, @ClientId={ClientId}").ToListAsync();
            await _cacheService.RemoveAsync(string.Format(CacheKeys.USER_PATTERN_KEY));
            return response[0];
        }

        public async Task<UResponse> ChangePasswordAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.ChangePassword}, @ClientId={user.ClientId}, @UserId={user.UserId}, @UserName={user.UserName}, @Password={user.Password}, @OldPassword={user.OldPassword}").ToListAsync();
            return response[0];
        }

        public async Task<List<UUserList>> GetUsersListAsync(int clientId, string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.USER_PATTERN_KEY, clientId, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.UserLists.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.List}, @ClientId={clientId}, @SearchStr={searchStr}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });
            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }

        public async Task<UResponse> AddUserTokenAsync(UserDto user)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.AddUserToken}, @ClientId={user.ClientId}, @UserId={user.UserId}, @AccessToken={user.AccessToken}, @RefreshToken={user.RefreshToken}, @RefreshTokenExpiry={user.RefreshTokenExpiry}").ToListAsync();
            return response[0];
        }

        public async Task<UUserDetail> GetUserByIdAsync(int clientId, int userId)
        {
            var cacheKey = string.Format(CacheKeys.USER_BY_ID_KEY, clientId, userId);
           var  cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.UserDetails.FromSqlInterpolated($"exec usp_Users_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId}, @UserId={userId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response[0];
            });
            if(cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }
    }
}
