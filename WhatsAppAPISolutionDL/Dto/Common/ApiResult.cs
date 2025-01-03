namespace WhatsAppAPISolutionDL.Dto.Common
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public object Result { get; set; }
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
    }
    public class SyncResult
    {
        public bool success { get; set; }
        public object result { get; set; }
        public string message { get; set; } = "";
        public int statusCode { get; set; }
    }
    public class MediaResult
    {
        public string id { get; set; }
        public string mediaId { get; set; }
    }

    public class CustomIntegrationResult
    {
        public bool Sent { get; set; }
        public string PhoneNumber { get; set; }
        public string WaId { get; set; }
        public string Errors { get; set; }
    }
}
