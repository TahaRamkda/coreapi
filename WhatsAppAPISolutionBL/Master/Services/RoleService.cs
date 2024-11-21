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

        public async Task<List<Role>> GetRoleListAsync(int clientId)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}", (int)CrudEnum.List, clientId);
            var response = await _dbContext.Roles.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddRoleAsync(RoleDto role)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}, @RoleName='{2}', @ActionBy={3}", (int)CrudEnum.Add, role.Client_Id, role.Role_Name, role.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> UpdateRoleAsync(RoleDto role)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @ClientId={1}, @RoleId={2}, @RoleName='{3}', @ActionBy={4}", (int)CrudEnum.Update, role.Client_Id, role.Role_Id, role.Role_Name, role.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteRoleAsync(int roleId, int clientId)
        {
            var query = string.Format(@"exec usp_Roles_Ops @ActionId={0}, @Role_Id={1}, @Client_Id={2}", (int)CrudEnum.Delete, roleId, clientId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
    }
}
