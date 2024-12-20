using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class USenderName
    {
        public int SenderId { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public string SenderName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberId { get; set; }
        public string BusinessAccountId { get; set; }
        public decimal? Limit { get; set; }
        public string Quality { get; set; }
        public int? LogoMediaId { get; set; }
        public string MediaPath { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int? Total { get; set; }
    }
}
