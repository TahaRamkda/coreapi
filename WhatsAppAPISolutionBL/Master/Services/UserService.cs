using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class UserService : IUserService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly WhatsAppSolutionContext _dbContext;

        public UserService(WhatsAppSolutionContext2 dbContext2, WhatsAppSolutionContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public Task<List<User>> GetUserListAsync(int ClientId)
        {
            throw new NotImplementedException();
        }

        public async Task<UUser> Login(string UserName, string Password, int RoleId = 0)
        {
            var query = string.Format(@"exec usp_Users_Login @UserName='{0}', @Password='{1}', @RoleId={2}", UserName, Password, RoleId);
            var userDetails = await _dbContext2.Users.FromSqlRaw(query).IgnoreQueryFilters().ToListAsync();
            if (userDetails != null && userDetails[0].Status > 0)
                userDetails[0].Permission = await _dbContext2.Permissions.FromSqlRaw(@"exec usp_GetUserPermission @UserId={0}", userDetails[0].UserId).IgnoreQueryFilters().ToListAsync();

            return userDetails[0];
        }

        public Task<User> RegisterUser(int clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<UResponse> AddUserAsync(UserDto user)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @ClientId={1}, @UserName='{2}', @Password='{3}', @IsActive={4}, @FullName='{5}', @Action_By={6}, @UserRoles='{7}'", (int)CrudEnum.Add, user.ClientId, user.UserName, user.Password, user.IsActive, user.FullName, user.ActionBy, user.UserRoles);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<UResponse> UpdateUserAsync(UserDto user)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @ClientId={1}, @UserId={2}, @UserName='{3}', @IsActive={4}, @FullName='{5}', @Action_By={6}, @UserRoles='{7}'", (int)CrudEnum.Update, user.ClientId, user.UserId, user.UserName, user.IsActive, user.FullName, user.ActionBy, user.UserRoles);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<UResponse> DeleteUserAsync(int UserId, int ClientId)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @UserId={1}, @ClientId={2}", (int)CrudEnum.Delete, UserId, ClientId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<UResponse> ChangePasswordAsync(UserDto user)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @ClientId={1}, @UserId={2}, @UserName='{3}', @Password='{4}', @OldPassword='{5}'", (int)CrudEnum.ChangePassword, user.ClientId, user.UserId, user.UserName, user.Password, user.OldPassword);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

        public async Task<List<User>> GetUsersListAsync(int ClientId)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, ClientId);
            var response = await _dbContext.Users.FromSqlRaw(query).ToListAsync();

            return response;
        }

        public async Task<UResponse> AddUserTokenAsync(UserDto user)
        {
            var query = string.Format(@"exec usp_Users_Ops @ActionId={0}, @ClientId={1}, @UserId={2}, @AccessToken='{3}', @RefreshToken='{4}', @RefreshTokenExpiry='{5}'", (int)CrudEnum.AddUserToken, user.ClientId, user.UserId, user.AccessToken, user.RefreshToken, user.RefreshTokenExpiry);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }

    }
}
