using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Contact
{
    public partial class UContact : UListWithBaseEntity
    {
        public int ContactId { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string AreaName { get; set; }
    }
}
