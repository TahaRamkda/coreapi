using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Employee
    {
        public long EmployeeId { get; set; }
        public int? ClientId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? JoinedDate { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? SeparationDate { get; set; }
        public decimal? BalanceAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public bool? Isactive { get; set; }
        public int? RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
