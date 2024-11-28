using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.Dto
{
    public partial class MediaFileDto
    {
        public int ClientId { get; set; }
        public int SenderNameId { get; set; }
        public IFormFile File { get; set; }
        public int ActionBy { get; set; }
    }
}
