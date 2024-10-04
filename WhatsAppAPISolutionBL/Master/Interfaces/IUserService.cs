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
        public Task<List<User>> GetUserListAsync(int clientId);
        public Task<User> RegisterUser(int clientId);
        public  Task<UUser> Login(string userName,string password);
        public Task<UResponse> AddUserAsync(UserDto user);
        public Task<UResponse> UpdateUserAsync(UserDto user);
        public Task<UResponse> AddUserTokenAsync(UserDto user);
        public Task<UResponse> DeleteUserAsync(int user_Id, int clientId);
        public Task<UResponse> ChangePasswordAsync(UserDto user);
        public Task<List<User>> GetUsersListAsync(int clientId);
    }
}
