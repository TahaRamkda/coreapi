using WhatsAppAPISolutionDL.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Flow;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IExportManager
    {
        //byte[] ExportBankAccountsToXlsx(IEnumerable<UBankAccount> bankAccount);
        //byte[] ExportAssetsToXlsx(IEnumerable<UAssets> assets);
        byte[] ExportConversationDetailReportToXlsx(IEnumerable<UConversationReportList> report);
        byte[] ExportAgentDetailSupervisorReportToXlsx(IEnumerable<UAgentSupervisorReport> report);
        byte[] ExportConversationReportToXlsx(IEnumerable<UConversationReportList> report);
        byte[] ExportAgentSupervisorReportToXlsx(IEnumerable<UAgentSupervisorReport> report);
        byte[] ExportSurveyResponseToXlsx(IEnumerable<USurveyResponse> surveyResponses);
    }
}
