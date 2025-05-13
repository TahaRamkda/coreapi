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
using WhatsAppAPISolutionBL.Helper;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICacheService _cacheService;

        public PermissionService(
            WhatsAppSolutionContext2 dbContext2,
            ICacheService cacheService)
        {
            _dbContext2 = dbContext2;
            _cacheService = cacheService;
        }

        public async Task<List<UPermissionList>> GetPermissionListAsync(int ClientId, int RoleId = 0)
        {
            var cacheKey = string.Format(CacheKeys.PERMISSION_DROPDOWN_KEY, ClientId, RoleId);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.PermissionsList.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @RoleId={RoleId}").ToListAsync();
                if (response == null || response.Count == 0)
                    return null;
                return response;
            });

            if (cacheResult == null || cacheResult.Count == 0)
                await _cacheService.RemoveAsync(cacheKey);

            return cacheResult;
        }

        public async Task<UResponse> AddPermissionAsync(int clientId, int userId, PermissionDto permission)
        {
            var permissionJson = JsonSerializer.Serialize(permission.Permissions);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Permission_Ops @ActionId={(int)CrudEnum.Update}, @ClientId={clientId}, @RoleId={permission.RoleId}, @PermissionJson={permissionJson}, @ActionBy={userId}").ToListAsync();

            await _cacheService.RemoveByPrefix(CacheKeys.PERMISSION_PATTERN_KEY);

            return response[0];
        }
    }
}
