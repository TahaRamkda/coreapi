using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppAPISolutionDL.UserModels.EnquirySubmission
{
    public class UEnquirySubmissionList
    {
        public int SubmissionId { get; set; }
        public int ClientId { get; set; }
        public int SenderId { get; set; }
        public int EnquiryId { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedDate { get; set; } // formatted date from SP
        public int Status { get; set; }
        public string EnquiryData { get; set; } // JSON string from SP
        public string SubmissionName { get; set; }
        public string EnquiryName { get; set; }
        public int StartTemplateId { get; set; }
        public int StartTemplateTypeId { get; set; }
        public int EndTemplateId { get; set; }
        public int EndTemplateTypeId { get; set; }
        public string EnquiryCreatedDate { get; set; } // formatted date from SP
        public long Line { get; set; }
        public int TotalRecords { get; set; } 
    }
}
