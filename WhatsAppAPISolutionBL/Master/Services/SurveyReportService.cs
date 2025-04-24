using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.SurveyReport;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class SurveyReportService : ISurveyReportService
    {
      private readonly WhatsAppSolutionContext2 _dbcontext2;
      private readonly ILogger<SurveyReportService> _logger;

        public SurveyReportService(WhatsAppSolutionContext2 dbcontext2, ILogger<SurveyReportService> logger)
        {
            _dbcontext2 = dbcontext2;
            _logger = logger;
        }
        public async Task<List<USurveyReport>> GetSurveyReportAsync(int? clientId, int? senderId, int? flowId, string phoneNumber, string searchText)
        {
            var StartProcTime = DateTime.Now;
            var response = await _dbcontext2.SurveyReports.FromSqlInterpolated($"Exec usp_GetSurveyReport @clientId={clientId}, @senderId={senderId}, @flowId={flowId}, @phoneNumber={phoneNumber}, @searchText={searchText}").ToListAsync();
            _logger.LogInformation("Calling usp_GetSurveyReport with the parameter clientId ={clientId}, senderId = {senderId}, flowId={flowId}, phoneNumber={phoneNumber}, searchText={searchText} and {ProcedureTime}" , clientId, senderId, flowId, phoneNumber, searchText, (DateTime.Now - StartProcTime).TotalMilliseconds);
            return response;
        }
    }
}
