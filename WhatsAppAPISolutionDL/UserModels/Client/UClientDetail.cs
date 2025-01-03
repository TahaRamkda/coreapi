namespace WhatsAppAPISolutionDL.UserModels.Client
{
    public class UClientDetail
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int? ClientLanguage { get; set; }
        public string ClientAddress { get; set; }
        public decimal? Balance { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhone { get; set; }
        public decimal? AvailableBalance { get; set; }
        public decimal? BalanceAlertLimit { get; set; }
        public string BusinessId { get; set; }
        public string AppId { get; set; }
        public string DefaultMarket { get; set; }
        public string Prefix { get; set; }
        public string Timezone { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
