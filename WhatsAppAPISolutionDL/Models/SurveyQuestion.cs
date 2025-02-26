using System;
using System.Collections.Generic;

namespace WhatsAppAPISolutionDL.Models
{
    public partial class SurveyQuestion
    {
        public int SurveyQuestionId { get; set; }
        public int? SurveyId { get; set; }
        public int? FlowId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
    }
}
