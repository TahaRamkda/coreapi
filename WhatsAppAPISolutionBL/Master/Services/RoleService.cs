using Microsoft.EntityFrameworkCore;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.User;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionDL.UserModels.User;

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

        public async Task<List<URole>> GetRoleListAsync(int ClientId)
        {
            var response = await _dbContext2.Roles.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddRoleAsync(RoleDto role)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={role.ClientId}, @RoleName={role.RoleName}, @ActionBy={role.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateRoleAsync(RoleDto role)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={role.ClientId}, @RoleId={role.RoleId}, @RoleName={role.RoleName}, @ActionBy={role.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteRoleAsync(int RoleId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Delete}, @RoleId={RoleId}").ToListAsync();
            return response[0];
        }

        public async Task<List<UEntityDto>> GetRolesAsync(int clientId, string searchStr = "")
        {
            var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
            return response;
        }

        public async Task<URoleDetail> GetRoleByIdAsync(int clientId, int roleId)
        {
            var response = await _dbContext2.RoleDetails.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId},@RoleId={roleId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
