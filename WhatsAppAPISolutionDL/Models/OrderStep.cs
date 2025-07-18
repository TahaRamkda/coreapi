using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class OrderStep
    {
        public int OrderStepId { get; set; }
        public int? OrderId { get; set; }
        public int? ClientId { get; set; }
        public int? SenderId { get; set; }
        public string PhoneNumber { get; set; }
        public int? StepTypeId { get; set; }
        public string StepName { get; set; }
        public int? OrderItemId { get; set; }
        public int? ItemId { get; set; }
        public int? FlowId { get; set; }
        public string FlowToken { get; set; }
        public int? Status { get; set; }
        public string Language { get; set; }
        public int? Sequence { get; set; }
        public string ButtonText { get; set; }
        public string BodyText { get; set; }
        public int? Priority { get; set; }
        public string ItemName { get; set; }
    }
}
