namespace WhatsAppAPISolutionDL.UserModels.Entity
{
    public class UEntity
    {
        public int? CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class UListEntity
    {
        public long? Line { get; set; }
        public int? TotalRecords { get; set; }
    }

    public class UListWithBaseEntity : UEntity
    {
        public long? Line { get; set; }
        public int? TotalRecords { get; set; }

    }
}
