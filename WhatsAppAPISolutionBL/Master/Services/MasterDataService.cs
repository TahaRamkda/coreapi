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

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class MasterDataService: IMasterDataService
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public MasterDataService(
            WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }

        public async Task<List<UMasterData>> GetMasterDataListAsync(string type)
        {
            var response = await _dbContext2.MasterData.FromSqlInterpolated($"exec usp_MasterData_Ops @ActionId={(int)CrudEnum.List}, @Type={type}").ToListAsync();
            return response;
        }
    }
}
