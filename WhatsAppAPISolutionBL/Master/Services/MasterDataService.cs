using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.MasterData;
using WhatsAppAPISolutionDL.Dto.Master;
using WhatsAppAPISolutionDL.Dto.Group;
using WhatsAppAPISolutionDL.UserModels.Entity;
using WhatsAppAPISolutionBL.Helper;


namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MasterDataService: IMasterDataService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ICacheService _cacheService;

        public MasterDataService(
            WhatsAppSolutionContext2 dbContext2,
            ICacheService cacheService)
        {
            _dbContext2 = dbContext2;
            _cacheService = cacheService;
        }

        public async Task<List<UMasterData>> GetMasterDataListAsync(string type)
        {
            var cacheKey = string.Format(CacheKeys.MASTERDATA_DROPDOWN_KEY, type);
            var cacheResult = await _cacheService.GetAsync(cacheKey, async () =>
            {
                var response = await _dbContext2.MasterData.FromSqlInterpolated($"exec usp_MasterData_Ops @ActionId={(int)CrudEnum.List}, @Type={type}").ToListAsync();
                if (response == null || response.Count == 0)
                {
                    return null;
                }
                return response;
            });
            if (cacheResult == null)
                await _cacheService.RemoveAsync(cacheKey);
            return cacheResult;
        }
        public async Task<UResponse> AddMasterDataAsync(MasterDto masterData)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($@"EXEC usp_MasterData_Ops @ActionId = {(int)CrudEnum.Add},@Mast_Id = {masterData.Mast_Id},@Name = {masterData.Name},@Type = {masterData.Type}").ToListAsync();
            await _cacheService.RemoveAsync(CacheKeys.MASTERDATA_PATTERN_KEY);
            return response.FirstOrDefault(); 
        }

        public async Task<UResponse> UpdateMasterDataAsync(MasterDto masterData)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($@"EXEC usp_MasterData_Ops @ActionId = {(int)CrudEnum.Update}, @Mast_Id = {masterData.Mast_Id},  @Name = {masterData.Name}, @Type = {masterData.Type}").ToListAsync();
            await _cacheService.RemoveAsync(CacheKeys.MASTERDATA_PATTERN_KEY);
            return response.FirstOrDefault();
        }


        public async Task<UResponse> DeleteMasterDataAsync(int mastId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($@"EXEC usp_MasterData_Ops @ActionId = {(int)CrudEnum.Delete}, @Mast_Id = {mastId}").ToListAsync();
            await _cacheService.RemoveAsync(CacheKeys.MASTERDATA_PATTERN_KEY);
            return response.FirstOrDefault();
        }


    }
}
