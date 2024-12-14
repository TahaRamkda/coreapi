using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class AgentTiming
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? AgentId { get; set; }
        public int? WeekDay { get; set; }
        public string WeekDayName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
