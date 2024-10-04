using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly WhatsAppAPISolutionContext2 _dbContext2;

        public PermissionService(WhatsAppAPISolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<List<UPermissionList>> GetPermissionListAsync(int client_Id, int role_Id = 0)
        {
            var query = string.Format(@"exec usp_Permission_Ops @ActionId={0}, @Client_Id={1}, @RoleId={2}", (int)CrudEnum.List, client_Id, role_Id);
            var response = await _dbContext2.PermissionsList.FromSqlRaw(query).ToListAsync();

            return response;
        }

        public async Task<UResponse> AddPermissionAsync(PermissionDto permission)
        {
            var permissionJson = JsonSerializer.Serialize(permission.Permissions);

            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.Update}, @Client_Id={permission.Client_Id}, @RoleId={permission.Role_Id}, @PermissionJson={permissionJson}, @Action_By={permission.ActionBy}").ToListAsync();

            return response[0];
        }
    }
}
