namespace WhatsAppAPISolutionDL.UserModels.User
{
    public partial class UUserDetail
    {
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public bool? IsActive { get; set; }
        public string RoleIds { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}
