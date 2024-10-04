using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Employe
    {
        public long EmployeId { get; set; }
        public int? ClientId { get; set; }
        public string? EmployeName { get; set; }
        public DateTime? JoinedDate { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? SeparationDate { get; set; }
        public bool? Isactive { get; set; }
        public decimal? BalanceAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
