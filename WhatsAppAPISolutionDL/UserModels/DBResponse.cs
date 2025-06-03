namespace WhatsAppAPISolutionDL.UserModels
{
    public partial class DBResponse
    {
        public int ResponseType { get; set; }
        public int? IsAutoResponse { get; set; }
        public string Json { get; set; }
    }
}
