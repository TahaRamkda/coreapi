using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class ClientDto
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
        public string AccessToken { get; set; }
        public int? ActionBy { get; set; }
    }
}
