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
        public string PhoneNumberId { get; set; }
        public string BusinessAccountId { get; set; }
        public decimal? Limit { get; set; }
        public string Quality { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? LogoMediaId { get; set; }
        public bool? Verified { get; set; }
    }
}
