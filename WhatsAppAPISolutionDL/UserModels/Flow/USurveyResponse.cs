using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Flow
{
    public class USurveyResponse
    {
        public string MetaFlowId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string SenderName { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public string CreatedDate { get; set; }  
    }
}
