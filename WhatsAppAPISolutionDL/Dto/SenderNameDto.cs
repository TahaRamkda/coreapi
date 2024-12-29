using Microsoft.AspNetCore.Http;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class SenderNameDto
    {
        public int SenderId { get; set; }
        public int ClientId { get; set; }
        public string SenderName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneId { get; set; }
        public string AppId { get; set; }
        public decimal Limit { get; set; }
        public bool Verified { get; set; }
        public string Quality { get; set; }
        public int MediaId { get; set; }
        public int ActionBy { get; set; }
        public IFormFile File { get; set; }
    } 
}
