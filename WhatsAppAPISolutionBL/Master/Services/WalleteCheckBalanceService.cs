using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.CheckWallet;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class WalleteCheckBalanceService : IWalleteCheckBalanceService
    {
        private readonly ILogger<WalleteCheckBalanceService> _logger;
        private readonly WhatsAppSolutionContext2 _dbContext;
        public WalleteCheckBalanceService(ILogger<WalleteCheckBalanceService> logger,
            WhatsAppSolutionContext2 dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<UWalletBalanceCheck> GetWalletBalanceAsync(int clientId)
        {
            var result = await _dbContext.CheckWalleteBalance
                .FromSqlInterpolated($"EXEC usp_clients_checkwalletBalance @ClientId={clientId}")
                .ToListAsync();

            return result.FirstOrDefault();
        }

    }
}
