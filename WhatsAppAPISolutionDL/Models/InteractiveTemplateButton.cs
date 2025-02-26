using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class InteractiveTemplateButton
    {
        public int ButtonId { get; set; }
        public int? InteractiveTemplateId { get; set; }
        public int? ClientId { get; set; }
        public string ButtonText { get; set; }
        public string ButtonValue { get; set; }
        public int? ButtonType { get; set; }
        public int? Sequence { get; set; }
        public int? ActionId { get; set; }
        public int? ActionType { get; set; }
        public int? SystemActionId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? SystemActionSubId { get; set; }
    }
}
