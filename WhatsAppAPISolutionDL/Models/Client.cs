using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Client
    {
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public int? ClientLanguage { get; set; }
        public string ClientAddress { get; set; }
        public decimal? Balance { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public decimal? BalanceAlertLimit { get; set; }
        public string AccessToken { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
