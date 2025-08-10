using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.EnquirySubmission;

namespace WhatsAppAPISolutionBL.Master.Services
{

    public class EnquirySubmission : IEnquirySubmission
    {
        private readonly WhatsAppSolutionContext2 _dbContext2;
        public EnquirySubmission(WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext2 = dbContext2;
        }
        public async Task<List<UEnquirySubmissionList>> GetEnquirySubmissionListsAsync(int clientId, int senderId, int enquiryId,  int sortBy, DateTime? FromDate, DateTime? ToDate, int pageNo=0, int pageSize=1000)
        {
            return await _dbContext2.EnquirySubmissionList.FromSqlInterpolated($"EXEC [dbo].[usp_GetEnquirySubmission_Ops] @ActionId={(int)CrudEnum.List}, @ClientId={clientId},@SenderId={senderId},@EnquiryId={enquiryId},@PageNo={pageNo},@pageSize={pageSize},@SortBy={sortBy}, @FromDate={FromDate}, @ToDate={ToDate}").ToListAsync();
        }
        public async Task<List<UEnquirySubmissionDetails>> GetUEnquirySubmissionDetails(int clientId,int senderId, string SearchStr)
        {
            return await _dbContext2.EnquirySubmissionDetails.FromSqlInterpolated($"EXEC [dbo].[usp_GetEnquirySubmission_Ops] @ActionId={(int)CrudEnum.GetEnquiryDetails}, @ClientId={clientId}, @SenderId={senderId}, @SearchStr={SearchStr}").ToListAsync();
        }
    }
}
