using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Media
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string WhatsAppBusinessAccountId { get; set; }
        public int? SenderNameId { get; set; }
        public string MediaUrl { get; set; }
        public string MediaId { get; set; }
        public string MediaPath { get; set; }
        public int? RecordStatus { get; set; }
        public string ContentType { get; set; }
        public int? FileSize { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int? MediaSourceId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? MediaTypeId { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
