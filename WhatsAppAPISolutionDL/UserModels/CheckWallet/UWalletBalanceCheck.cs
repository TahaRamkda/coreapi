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
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public string ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }
        public string CountryCode { get; set; }     
        public string CurrencyCode { get; set; }    
        public string Status { get; set; }

    }
}
