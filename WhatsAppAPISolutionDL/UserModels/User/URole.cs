namespace WhatsAppAPISolutionDL.UserModels.User
{
    public class URole
    {
        public int RoleId { get; set; }
        public int? ClientId { get; set; }
        public string RoleName { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int? MasterRole { get; set; }
    }
}
