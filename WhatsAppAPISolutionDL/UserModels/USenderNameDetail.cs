namespace WhatsAppAPISolutionDL.UserModels
{
    public class USenderNameDetail
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
        public bool? Verified { get; set; }
        public string MediaPath { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
