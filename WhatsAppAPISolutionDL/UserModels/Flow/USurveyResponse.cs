using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.Flow
{
    public class USurveyResponse
    {
        public int SurveyResponseId { get; set; }
        public int? SurveyId { get; set; }
        public int? FlowId { get; set; }
        public string MetaFlowId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        //public string FlowToken { get; set; }
        public int? SenderId { get; set; }
        public int? ClientId { get; set; }
        public int? ModuleId { get; set; }
        public int? ParentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<USurveyResponseDetail> SurveyResponseDetails { get; set; }
    }

    public class USurveyResponseDetail
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
