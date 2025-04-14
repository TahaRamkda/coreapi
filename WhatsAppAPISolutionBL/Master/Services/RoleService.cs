using Microsoft.EntityFrameworkCore;
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
    public class RoleService : IRoleService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICacheService _cacheService;
        public RoleService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ICacheService cacheService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _cacheService = cacheService;
        }

        public async Task<List<URole>> GetRoleListAsync(int ClientId)
        {
            var cacheKey = string.Format(CacheKeys.ROLE_DROPDOWN_KEY, ClientId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.Roles.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });
            if (cacheResult == null || cacheResult.Count == 0)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }

        public async Task<UResponse> AddRoleAsync(int clientId, int userId, RoleDto role)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={clientId}, @RoleName={role.RoleName}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> UpdateRoleAsync(int clientId, int userId, RoleDto role)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={clientId}, @RoleId={role.RoleId}, @RoleName={role.RoleName}, @ActionBy={userId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteRoleAsync(int RoleId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.Delete}, @RoleId={RoleId}").ToListAsync();
            await _cacheService.RemoveAsync(CacheKeys.ROLE_PATTERN_KEY);
            return response[0];
        }

        public async Task<List<UEntityDto>> GetRolesAsync(int clientId, string searchStr = "")
        {
            var cacheKey = string.Format(CacheKeys.ROLE_DROPDOWN_KEY2, clientId, searchStr);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.Entity.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.GetEntities}, @ClientId={clientId},  @SearchStr={searchStr}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });
            if (cacheResult == null || cacheResult.Count == 0)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }

        public async Task<URoleDetail> GetRoleByIdAsync(int clientId, int roleId)
        {
            var cacheKey = string.Format(CacheKeys.ROLE_BY_ID_KEY, clientId, roleId);
            var cacheResult = _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.RoleDetails.FromSqlInterpolated($"exec usp_Roles_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId},@RoleId={roleId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response[0];
            });
            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return await cacheResult;
        }
    }
}
