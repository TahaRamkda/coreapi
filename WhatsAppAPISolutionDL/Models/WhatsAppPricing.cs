using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class WhatsAppPricing
    {
        public int PricingId { get; set; }
        public string Market { get; set; }
        public string Currency { get; set; }
        public decimal? Marketing { get; set; }
        public decimal? MarketingCommission { get; set; }
        public decimal? Utility { get; set; }
        public decimal? UtilityCommission { get; set; }
        public decimal? Authentication { get; set; }
        public decimal? AuthenticationCommission { get; set; }
        public decimal? AuthenticationInternational { get; set; }
        public decimal? AuthenticationInternationalCommission { get; set; }
        public decimal? Service { get; set; }
        public decimal? ServiceCommission { get; set; }
        public int? ClientId { get; set; }
    }
}
