using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Client
    {
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? ClientLanguage { get; set; }
        public string ClientPrefix { get; set; }
        public int? StickerPerPageItem { get; set; }
        public int? PrintingPerPageItem { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
