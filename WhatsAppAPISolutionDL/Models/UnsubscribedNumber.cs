using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class UnsubscribedNumber
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string BlockType { get; set; }
        public int? ClientId { get; set; }
        public int? ParentId { get; set; }
        public int? TemplateId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
