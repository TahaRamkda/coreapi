using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Group
{
    public partial class UGroup : UListWithBaseEntity
    {
        public int GroupId { get; set; }
        public int ClientId { get; set; }
        public string GroupName { get; set; }
        public int TotalContacts { get; set; }
    }
}
