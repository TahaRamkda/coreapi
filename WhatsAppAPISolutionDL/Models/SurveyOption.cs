using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyOption
    {
        public int SurveyOptionId { get; set; }
        public int? SurveyQuestionId { get; set; }
        public string OptionId { get; set; }
        public string OptionText { get; set; }
        public bool? Status { get; set; }
    }
}
