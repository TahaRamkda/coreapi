using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyResponseDetail
    {
        public long SurveyResponseDetailId { get; set; }
        public long? SurveyResponseId { get; set; }
        public string OptionId { get; set; }
        public string OptionText { get; set; }
        public string QuestionText { get; set; }
        public int? SurveyQuestionId { get; set; }
    }
}
