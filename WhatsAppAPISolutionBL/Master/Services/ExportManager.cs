using OfficeOpenXml;
using OfficeOpenXml.Style;
using WhatsAppAPISolutionBL.Helper; 
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Flow;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ExportManager : IExportManager
    {
        public virtual byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var stream = new MemoryStream())
            {
                using (var xlPackage = new ExcelPackage(stream))
                {
                    //var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                    var manager = new PropertyManager<T>(properties);
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++);
                    }
                    worksheet.Cells.AutoFitColumns();
                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }
        private void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }


        public virtual byte[] ExportConversationDetailReportToXlsx(IEnumerable<UConversationReportList> report)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UConversationReportList>("SenderName", p => p.SenderName),
                new PropertyByName<UConversationReportList>("TotalMessages", p => p.TotalMessages),
                new PropertyByName<UConversationReportList>("FullName", p => p.FullName),
                new PropertyByName<UConversationReportList>("PhoneNumber", p => p.PhoneNumber),
                new PropertyByName<UConversationReportList>("MessageText", p => p.LastMessageText),
                new PropertyByName<UConversationReportList>("AgentName", p => p.AgentName),
                new PropertyByName<UConversationReportList>("StatusName", p => p.StatusName),
                new PropertyByName<UConversationReportList>("UnreadCount", p => p.UnreadCount),
                new PropertyByName<UConversationReportList>("CreatedDate", p => p.CreatedDate),
                new PropertyByName<UConversationReportList>("UpdatedDate", p => p.UpdatedDate),
                new PropertyByName<UConversationReportList>("ExpiryDate", p => p.ExpiryDate)
            };

            return ExportToXlsx(properties, report);
        }

        public virtual byte[] ExportAgentDetailSupervisorReportToXlsx(IEnumerable<UAgentSupervisorReport> report)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UAgentSupervisorReport>("AgentName", p => p.AgentName),
                new PropertyByName<UAgentSupervisorReport>("ActiveChat", p => p.ActiveChat),
                new PropertyByName<UAgentSupervisorReport>("AssignedChat", p => p.AssignedChat),
                new PropertyByName<UAgentSupervisorReport>("UnAssignedChat", p => p.UnAssignedChat),
                new PropertyByName<UAgentSupervisorReport>("AbandonChat", p => p.AbandonChat),
                new PropertyByName<UAgentSupervisorReport>("ExpiredChat", p => p.ExpiredChat),
                new PropertyByName<UAgentSupervisorReport>("ForceClosedChat", p => p.ForceClosedChat),
                new PropertyByName<UAgentSupervisorReport>("ClosedChat", p => p.ClosedChat),
                new PropertyByName<UAgentSupervisorReport>("Rating", p => p.Rating),
                new PropertyByName<UAgentSupervisorReport>("AvgResponseTime", p => p.AvgResponseTime),
                new PropertyByName<UAgentSupervisorReport>("AvgChatTime", p => p.AvgChatTime),
                new PropertyByName<UAgentSupervisorReport>("IsDisabled", p => p.IsDisabled),
                new PropertyByName<UAgentSupervisorReport>("StatusName", p => p.StatusName)
            };

            return ExportToXlsx(properties, report);
        }

        public virtual byte[] ExportConversationReportToXlsx(IEnumerable<UAgentSupervisorReport> report)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UAgentSupervisorReport>("AgentName", p => p.AgentName),
                new PropertyByName<UAgentSupervisorReport>("ActiveChat", p => p.ActiveChat),
                new PropertyByName<UAgentSupervisorReport>("AssignedChat", p => p.AssignedChat),
                new PropertyByName<UAgentSupervisorReport>("UnAssignedChat", p => p.UnAssignedChat),
                new PropertyByName<UAgentSupervisorReport>("AbandonChat", p => p.AbandonChat),
                new PropertyByName<UAgentSupervisorReport>("ExpiredChat", p => p.ExpiredChat),
                new PropertyByName<UAgentSupervisorReport>("ForceClosedChat", p => p.ForceClosedChat),
                new PropertyByName<UAgentSupervisorReport>("ClosedChat", p => p.ClosedChat),
                new PropertyByName<UAgentSupervisorReport>("Rating", p => p.Rating),
                new PropertyByName<UAgentSupervisorReport>("AvgResponseTime", p => p.AvgResponseTime),
                new PropertyByName<UAgentSupervisorReport>("AvgChatTime", p => p.AvgChatTime),
                new PropertyByName<UAgentSupervisorReport>("IsDisabled", p => p.IsDisabled),
                new PropertyByName<UAgentSupervisorReport>("StatusName", p => p.StatusName)
            };

            return ExportToXlsx(properties, report);
        }

        public virtual byte[] ExportConversationReportToXlsx(IEnumerable<UConversationReportList> report)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UConversationReportList>("SenderName", p => p.SenderName),
                new PropertyByName<UConversationReportList>("TotalMessages", p => p.TotalMessages),
                new PropertyByName<UConversationReportList>("FullName", p => p.FullName),
                new PropertyByName<UConversationReportList>("PhoneNumber", p => p.PhoneNumber),
                new PropertyByName<UConversationReportList>("MessageText", p => p.LastMessageText),
                new PropertyByName<UConversationReportList>("AgentName", p => p.AgentName),
                new PropertyByName<UConversationReportList>("StatusName", p => p.StatusName),
                new PropertyByName<UConversationReportList>("UnreadCount", p => p.UnreadCount),
                new PropertyByName<UConversationReportList>("CreatedDate", p => p.CreatedDate),
                new PropertyByName<UConversationReportList>("UpdatedDate", p => p.UpdatedDate),
                new PropertyByName<UConversationReportList>("ExpiryDate", p => p.ExpiryDate)
            };

            return ExportToXlsx(properties, report);
        }

        public virtual byte[] ExportAgentSupervisorReportToXlsx(IEnumerable<UAgentSupervisorReport> report)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UAgentSupervisorReport>("AgentName", p => p.AgentName),
                new PropertyByName<UAgentSupervisorReport>("ActiveChat", p => p.ActiveChat),
                new PropertyByName<UAgentSupervisorReport>("AssignedChat", p => p.AssignedChat),
                new PropertyByName<UAgentSupervisorReport>("UnAssignedChat", p => p.UnAssignedChat),
                new PropertyByName<UAgentSupervisorReport>("AbandonChat", p => p.AbandonChat),
                new PropertyByName<UAgentSupervisorReport>("ExpiredChat", p => p.ExpiredChat),
                new PropertyByName<UAgentSupervisorReport>("ForceClosedChat", p => p.ForceClosedChat),
                new PropertyByName<UAgentSupervisorReport>("ClosedChat", p => p.ClosedChat),
                new PropertyByName<UAgentSupervisorReport>("Rating", p => p.Rating),
                new PropertyByName<UAgentSupervisorReport>("AvgResponseTime", p => p.AvgResponseTime),
                new PropertyByName<UAgentSupervisorReport>("AvgChatTime", p => p.AvgChatTime),
                new PropertyByName<UAgentSupervisorReport>("IsDisabled", p => p.IsDisabled),
                new PropertyByName<UAgentSupervisorReport>("StatusName", p => p.StatusName)
            };

            return ExportToXlsx(properties, report);
        }

        public virtual byte[] ExportSurveyResponseToXlsx(IEnumerable<USurveyResponse> surveyResponses)
        {
            var flattenedData = surveyResponses
                .SelectMany(response => response.SurveyResponseDetails.Select(detail => new
                {
                    response.SurveyResponseId,
                    response.SurveyId,
                    response.FlowId,
                    response.MetaFlowId,
                    response.PhoneNumber,
                    response.Name,
                    response.SenderId,
                    response.ClientId,
                    response.ModuleId,
                    response.ParentId,
                    CreatedDate = response.CreatedDate.HasValue
                        ? CommonHelper.ConvertUtcToUserTimeZone(response.CreatedDate.Value, "+03:00")
                        : "",
                    QuestionText = detail.QuestionText,
                    AnswerText = detail.OptionText,
                    AnswerType = detail.Type
                })).ToList();

            // Define properties for the export
            var properties = new[]
            {
        new PropertyByName<dynamic>("SurveyResponseId", p => p.SurveyResponseId),
        new PropertyByName<dynamic>("SurveyId", p => p.SurveyId),
        new PropertyByName<dynamic>("FlowId", p => p.FlowId),
        new PropertyByName<dynamic>("MetaFlowId", p => p.MetaFlowId),
        new PropertyByName<dynamic>("PhoneNumber", p => p.PhoneNumber),
        new PropertyByName<dynamic>("Name", p => p.Name),
        new PropertyByName<dynamic>("SenderId", p => p.SenderId),
        new PropertyByName<dynamic>("ClientId", p => p.ClientId),
        //new PropertyByName<dynamic>("ModuleId", p => p.ModuleId),
        //new PropertyByName<dynamic>("ParentId", p => p.ParentId),
        new PropertyByName<dynamic>("CreatedDate", p => p.CreatedDate),
        new PropertyByName<dynamic>("Question", p => p.QuestionText),
        new PropertyByName<dynamic>("Answer", p => p.AnswerText)
    };

            return ExportToXlsx(properties, flattenedData);
        }

    }
}
