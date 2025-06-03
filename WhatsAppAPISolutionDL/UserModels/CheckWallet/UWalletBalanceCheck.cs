using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.Enum;

namespace WhatsAppAPISolutionDL.UserModels.CheckWallet
{
    public class UWalletBalanceCheck
    {
        public SubscriptionType SubscriptionType { get; set; }
        public decimal Balance { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }

    }
}
