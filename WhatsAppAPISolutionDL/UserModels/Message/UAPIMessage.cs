using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionDL.UserModels.Message
{
    public partial class UAPIMessage : UListEntity
    {
        public int APIMessageId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderNameId { get; set; }
        public string TrxType { get; set; }
        public string UDF1 { get; set; }
        public string UDF2 { get; set; }
        public int? TemplateId { get; set; }
        public string PhoneNumber { get; set; }
        public string URL { get; set; }
        public int? Status { get; set; }
        public string WaId { get; set; }
        public string ScheduleTime { get; set; }
        public string CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
