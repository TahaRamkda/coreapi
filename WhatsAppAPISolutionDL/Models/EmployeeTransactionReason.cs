using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class EmployeeTransactionReason
    {
        public int Id { get; set; }
        public int ReasonId { get; set; }
        public string ReasonName { get; set; }
    }
}
