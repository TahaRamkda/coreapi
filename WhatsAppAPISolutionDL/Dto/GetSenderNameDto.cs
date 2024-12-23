namespace WhatsAppAPISolutionDL.Dto
{
    public partial class GetSenderNameDto
    {
        public int SenderId { get; set; }
        public int? ClientId { get; set; }
        public string SenderName1 { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneNumberId { get; set; }
        public string BusinessAccountId { get; set; }
        public decimal? Limit { get; set; }
        public string Quality { get; set; }
        public int LogoMediaId { get; set; } 
        public string LogoUrl { get; set; } 
    }
}
