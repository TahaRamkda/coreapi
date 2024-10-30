namespace WhatsAppAPISolutionAPI.Models
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public Object Result { get; set; }
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
    }
    public class SyncResult
    {
        public bool success { get; set; }
        public Object result { get; set; }
        public string message { get; set; } = "";
        public int statusCode { get; set; }
    }
    public class MediaResult
    {
        public string id { get; set; }
        public string mediaId { get; set; }
    }
}
