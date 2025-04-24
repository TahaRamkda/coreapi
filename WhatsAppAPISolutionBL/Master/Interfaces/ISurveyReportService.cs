using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.SurveyReport;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface ISurveyReportService
    {
        Task<List<USurveyReport>> GetSurveyReportAsync(int? clientId, int? senderId, int? flowId, string phoneNumber, string searchText);
    }
}
