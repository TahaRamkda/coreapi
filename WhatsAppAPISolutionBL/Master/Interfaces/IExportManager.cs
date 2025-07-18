using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Catalog;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;

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
        byte[] ExportCatalogItemsENToXlsx(IEnumerable<UCatalogExport> item);
        byte[] ExportCatalogItemsARToXlsx(IEnumerable<UCatalogExport> item);
        byte[] ExportCatalogItemsENToCsv(IEnumerable<UCatalogExport> item);
        byte[] ExportCatalogItemsARToCsv(IEnumerable<UCatalogExport> item);
    }
}
