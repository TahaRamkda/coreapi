using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetUserListAsync(int ClientId);
        public Task<User> RegisterUser(int ClientId);
        public Task<UUser> Login(string UserName, string Password, int MasterRoleTypeId = 0);
        public Task<UResponse> AddUserAsync(UserDto user);
        public Task<UResponse> UpdateUserAsync(UserDto user);
        public Task<UResponse> AddUserTokenAsync(UserDto user);
        public Task<UResponse> DeleteUserAsync(int UserId, int ClientId);
        public Task<UResponse> ChangePasswordAsync(UserDto user);
        public Task<List<User>> GetUsersListAsync(int ClientId);
    }
}
