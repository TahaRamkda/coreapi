namespace WhatsAppAPISolutionDL.Dto
{
    public class GetMediaResult
    {
        public string url { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public long file_size { get; set; }
        public string id { get; set; }
        public string message_product { get; set; }
    }
}
