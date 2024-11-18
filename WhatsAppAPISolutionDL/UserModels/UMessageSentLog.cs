namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class UMessageSentLog
    {
        public long Id { get; set; }
        public long? ModuleId { get; set; }
        public long? ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public string WaId { get; set; }
        public string WaId2 { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public string SentDate { get; set; }
        public int? SentStatus { get; set; }
        public string SentMessage { get; set; }
        public string DeliveredDate { get; set; }
        public int? DeliveredStatus { get; set; }
        public string DeliveredMessage { get; set; }
        public string ReadDate { get; set; }
        public int? ReadStatus { get; set; }
        public string ReadMessage { get; set; }
        public string PricingModel { get; set; }
        public bool? Billable { get; set; }
        public string Category { get; set; }
        public decimal? EstPrice { get; set; }
        public decimal? Commission { get; set; }
        public string CreatedDate { get; set; }
        public int CurrentStatus { get; set; }
        public string CurrentStatusName { get; set; }
        public long? Line { get; set; }
        public int? TotalItems { get; set; }
    }
}
