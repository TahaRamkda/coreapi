namespace WhatsAppAPISolutionDL.UserModels.User
{
    public partial class UUserList
    {
        public int UserId { get; set; }
        public int? ClientId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public string FullName { get; set; } 
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
