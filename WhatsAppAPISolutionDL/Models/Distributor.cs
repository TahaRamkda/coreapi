using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Distributor
    {
        public long DistributorId { get; set; }
        public int? ClientId { get; set; }
        public string DistributorName { get; set; }
        public decimal? PrintingBalance { get; set; }
        public decimal? StickerBalance { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Phone2 { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
