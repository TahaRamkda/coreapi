using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class VwCountryPricing
    {
        public int? CountryId { get; set; }
        public string Iso { get; set; }
        public string CountryName { get; set; }
        public string Nicename { get; set; }
        public string Iso3 { get; set; }
        public int? Numcode { get; set; }
        public int? Phonecode { get; set; }
        public string Fbmarket { get; set; }
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
        public decimal? Waorder { get; set; }
        public decimal? WaorderCommission { get; set; }
        public int? ClientId { get; set; }
        public decimal? UtilityCoin { get; set; }
        public decimal? MarketingCoin { get; set; }
        public decimal? AuthenticationCoin { get; set; }
        public decimal? ServiceCoin { get; set; }
        public decimal? WaorderCoin { get; set; }
    }
}
