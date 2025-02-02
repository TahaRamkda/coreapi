using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels.Permission;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.Dto.User;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly int userId;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IUserService _userService;

        public PermissionService(
            WhatsAppSolutionContext2 dbContext2,
            IUserService userService)
        {
            _dbContext2 = dbContext2;
            _userService = userService;


            userId = _userService.GetUserIdFromAccessToken();
        }

        public async Task<List<UPermissionList>> GetPermissionListAsync(int ClientId, int RoleId = 0)
        {
            var response = await _dbContext2.PermissionsList.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @RoleId={RoleId}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddPermissionAsync(PermissionDto permission)
        {
            var permissionJson = JsonSerializer.Serialize(permission.Permissions);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={permission.ClientId}, @RoleId={permission.RoleId}, @PermissionJson={permissionJson}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }
    }
}
