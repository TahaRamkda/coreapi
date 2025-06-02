using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.CheckWallet;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IWalleteCheckBalanceService
    {
            Task<UWalletBalanceCheck> GetWalletBalanceAsync(int clientId);
    }
}
