using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ClientWallet
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string Currency { get; set; }
        public decimal? Balance { get; set; }
        public bool IsPostpaid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool? IsCoin { get; set; }
    }
}
