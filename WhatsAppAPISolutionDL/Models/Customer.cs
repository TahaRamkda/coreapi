using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Customer
    {
        public long CustomerId { get; set; }
        public int? ClientId { get; set; }
        public string CustomerName { get; set; }
        public decimal? StickerBalance { get; set; }
        public decimal? PrintingBalance { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
    }
}
