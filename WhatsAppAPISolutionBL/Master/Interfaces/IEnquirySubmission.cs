using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.EnquirySubmission;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IEnquirySubmission
    {
        Task<List<UEnquirySubmissionList>> GetEnquirySubmissionListsAsync(int clientId, int senderId, int enquiryId, int sortBy, DateTime? fromDate, DateTime? ToDate, int pageNo=0, int pageSize=1000);
        Task<List<UEnquirySubmissionDetails>> GetUEnquirySubmissionDetails(int clientId, int senderId, string SearchStr);
    }
}
