using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SenderName
    {
        public int SenderId { get; set; }
        public int? ClientId { get; set; }
        public string SenderName1 { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneId { get; set; }
        public string AppId { get; set; }
        public decimal? Limit { get; set; }
        public decimal? Quality { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
