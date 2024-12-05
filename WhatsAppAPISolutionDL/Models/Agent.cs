using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class Agent
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string AgentFname { get; set; }
        public string AgentLname { get; set; }
        public int? Status { get; set; }
        public int? RecordStatus { get; set; }
        public DateTime? LastOnline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
