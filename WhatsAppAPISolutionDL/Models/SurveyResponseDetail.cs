using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyResponseDetail
    {
        public int SurveyResponseDetailId { get; set; }
        public int? SurveyResponseId { get; set; }
        public string OptionText { get; set; }
        public string QuestionText { get; set; }
        public string Type { get; set; }
        public string QuestionKey { get; set; }
        public string AnswerKey { get; set; }
    }
}
