using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.InteractiveTemplate
{
    public partial class UInteractiveTemplate : UListWithBaseEntity
    {
        public int InteractiveTemplateId { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string TemplateName { get; set; }
        public string Language { get; set; }
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public bool? UsedByAgent { get; set; }
        public int? DefaultTypeId { get; set; }
        public string DefaultType { get; set; }  
    }
}
