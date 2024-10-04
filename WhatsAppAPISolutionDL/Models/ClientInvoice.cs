using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class ClientInvoice
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public string InvoicePrefix { get; set; }
        public int? LastInvoiceNumber { get; set; }
        public int? InvoiceTypeId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PreviousUpdatedDate { get; set; }
    }
}
