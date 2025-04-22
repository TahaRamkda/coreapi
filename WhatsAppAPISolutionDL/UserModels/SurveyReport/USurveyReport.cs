using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.SurveyReport
{
    public class USurveyReport
    {
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string SurveyCreatedBy { get; set; }
        public DateTime SurveyCreatedDate { get; set; }
        public string PhoneNumber { get; set; }
        public string FlowToken { get; set; }
        public DateTime ResponseDate { get; set; }
        public string QuestionText { get; set; }
        public string OptionText { get; set; }
        public string Type { get; set; }
    }
}
