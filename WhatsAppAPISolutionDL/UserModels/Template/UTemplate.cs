using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Template
{
    public partial class UTemplate : UListEntity
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string TemplateName { get; set; }
        public string TemplateId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Language { get; set; }
        public string Status { get; set; } 
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
