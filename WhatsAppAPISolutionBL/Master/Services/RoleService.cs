using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class RoleService : IRoleService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public RoleService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<Role>> GetRoleListAsync(int ClientId)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, ClientId);
            var response = await _dbContext.Roles.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddRoleAsync(RoleDto role)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}, @RoleName='{2}', @ActionBy={3}", (int)CrudEnum.Add, role.ClientId, role.RoleName, role.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateRoleAsync(RoleDto role)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}, @RoleId={2}, @RoleName='{3}', @ActionBy={4}", (int)CrudEnum.Update, role.ClientId, role.RoleId, role.RoleName, role.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteRoleAsync(int RoleId)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @RoleId={1}", (int)CrudEnum.Delete, RoleId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
