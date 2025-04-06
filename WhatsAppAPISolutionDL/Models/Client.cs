using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Client
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
        public string BusinessId { get; set; }
        public string AppId { get; set; }
        public string AccessToken { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string DefaultMarket { get; set; }
        public string Timezone { get; set; }
        public string Prefix { get; set; }
        public int? Currency { get; set; }
    }
}
