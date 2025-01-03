using Microsoft.AspNetCore.Http;

namespace WhatsAppAPISolutionDL.Dto.Media
{
    public partial class MediaFileDto
    {
        public int ClientId { get; set; }
        public int SenderNameId { get; set; }
        public IFormFile File { get; set; }
        public bool UploadToFacebook { get; set; } = true;
        public int MediaSourceId { get; set; }
        public int ActionBy { get; set; }
    }
}
