using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string WaId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? Status { get; set; }
        public int? ErrorCount { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public string MetaOrderId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Language { get; set; }
    }
}
