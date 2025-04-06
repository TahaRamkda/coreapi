using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.SenderName
{
    public partial class USenderName : UListWithBaseEntity
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
        public string WebsiteUrl { get; set; }
    }
}
