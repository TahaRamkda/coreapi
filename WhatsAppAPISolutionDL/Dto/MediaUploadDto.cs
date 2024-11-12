using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class MediaUploadDto
    {
        public long Id { get; set; }
        public int ClientId { get; set; }
        public string WhatsAppBusinessAccountId { get; set; }
        public int SenderNameId { get; set; }
        public string MediaUrl { get; set; }
        public string MediaId { get; set; }
        public string MediaPath { get; set; }
        public IFormFile File { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int ActionBy { get; set; }
    }
}
