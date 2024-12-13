using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
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

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public PermissionService(WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<List<UPermissionList>> GetPermissionListAsync(int ClientId, int RoleId = 0)
        {
            var query = string.Format(@"exec usp_Permission_Ops @ActionId={0}, @ClientId={1}, @RoleId={2}", (int)CrudEnum.List, ClientId, RoleId);
            var response = await _dbContext2.PermissionsList.FromSqlRaw(query).ToListAsync();

            return response;
        }

        public async Task<UResponse> AddPermissionAsync(PermissionDto permission)
        {
            var permissionJson = JsonSerializer.Serialize(permission.Permissions);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={permission.ClientId}, @RoleId={permission.RoleId}, @PermissionJson={permissionJson}, @ActionBy={permission.ActionBy}").ToListAsync();

            return response[0];
        }
    }
}
