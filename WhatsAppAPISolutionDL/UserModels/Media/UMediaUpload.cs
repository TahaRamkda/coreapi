using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Media
{
    public partial class UMediaUpload
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string WhatsAppBusinessAccountId { get; set; }
        public int? SenderNameId { get; set; }
        public string SenderName { get; set; }
        public string MediaUrl { get; set; }
        public string MediaId { get; set; }
        public string MediaPath { get; set; }
        public string ContentType { get; set; }
        public int? FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}
