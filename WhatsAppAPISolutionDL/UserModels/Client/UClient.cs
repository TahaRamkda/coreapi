using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Client
{
    public partial class UClient : UListWithBaseEntity
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int? ClientLanguage { get; set; }
        public string ClientAddress { get; set; }
        public decimal? Balance { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public decimal? BalanceAlertLimit { get; set; }
        public string Prefix { get; set; }
        public string Timezone { get; set; }
        public int? Currency { get; set; }
        public string CurrencyName { get; set; }
    }
}
