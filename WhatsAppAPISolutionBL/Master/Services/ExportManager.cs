using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;
using System.Text;
using WhatsAppAPISolutionBL.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels.Agent;
using WhatsAppAPISolutionDL.UserModels.Catalog;
using WhatsAppAPISolutionDL.UserModels.Conversation;
using WhatsAppAPISolutionDL.UserModels.Flow;
using WhatsAppAPISolutionDL.UserModels.TemplateAnalytic;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ExportManager : IExportManager
    {
        #region Utilities

        private static byte[] ConvertToCsvBytes<T>(IEnumerable<T> records, ClassMap<T> classMap)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap(classMap);
            csv.WriteRecords(records);
            writer.Flush(); // Important: Flush before reading from MemoryStream

            return memoryStream.ToArray();
        }
 
        public class UCatalogExportMapEN : ClassMap<UCatalogExport>
        {
            public UCatalogExportMapEN()
            {
                Map(p => p.ItemId).Name("id");
                Map(p => p.ProductNameEn).Name("title");
                Map(p => p.DescriptionEn).Name("description");
                Map(p => p.Availability).Name("availability");
                Map(p => p.Condition).Name("condition");
                Map(p => p.Price).Name("price");
                Map(p => p.Link).Name("link");
                Map(p => p.ImageUrl).Name("image_link");
                Map(p => p.Brand).Name("brand");
                Map(p => p.CategoryNameEn).Name("google_product_category"); 
            }
        }

        public class UCatalogExportMapAR : ClassMap<UCatalogExport>
        {
            public UCatalogExportMapAR()
            {
                Map(p => p.ItemId).Name("id");
                Map(p => p.ProductNameAr).Name("title");
                Map(p => p.DescriptionAr).Name("description");
                Map(p => p.Availability).Name("availability");
                Map(p => p.Condition).Name("condition");
                Map(p => p.Price).Name("price");
                Map(p => p.Link).Name("link");
                Map(p => p.ImageUrl).Name("image_link");
                Map(p => p.Brand).Name("brand");
                Map(p => p.CategoryNameAr).Name("google_product_category");
            }
        }
         
        #endregion

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
                new PropertyByName<UConversationReportList>("ExpiryDate", p => p.ExpiryDate),
                new PropertyByName<UConversationReportList>("ReasonName", p => p.ReasonName)
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
                new PropertyByName<UConversationReportList>("ExpiryDate", p => p.ExpiryDate),
                new PropertyByName<UConversationReportList>("ReasonName", p => p.ReasonName)
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
            // Define properties for the export
            var properties = new[]
            {
                new PropertyByName<USurveyResponse>("MetaFlowId", p => p.MetaFlowId),
                new PropertyByName<USurveyResponse>("PhoneNumber", p => p.PhoneNumber),
                new PropertyByName<USurveyResponse>("Name", p => p.Name),
                new PropertyByName<USurveyResponse>("SenderName", p => p.SenderName),
                new PropertyByName<USurveyResponse>("Question", p => p.QuestionText),
                new PropertyByName<USurveyResponse>("Answer", p => p.AnswerText),
                new PropertyByName<USurveyResponse>("CreatedDate", p => p.CreatedDate)
            };

            return ExportToXlsx(properties, surveyResponses);
        }

        public virtual byte[] ExportCatalogItemsENToXlsx(IEnumerable<UCatalogExport> item)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UCatalogExport>("id", p => p.ItemId),
                new PropertyByName<UCatalogExport>("title", p => p.ProductNameEn),
                new PropertyByName<UCatalogExport>("description", p => p.DescriptionEn),
                new PropertyByName<UCatalogExport>("availability", p => p.Availability),
                new PropertyByName<UCatalogExport>("condition", p => p.Condition),
                new PropertyByName<UCatalogExport>("price", p => p.Price),
                new PropertyByName<UCatalogExport>("link", p => p.Link),
                new PropertyByName<UCatalogExport>("image_link", p => p.ImageUrl),
                new PropertyByName<UCatalogExport>("brand", p => p.Brand),
                new PropertyByName<UCatalogExport>("collection", p => p.CategoryNameEn),
                new PropertyByName<UCatalogExport>("collection_name", p => p.CategoryNameEn)
            };

            return ExportToXlsx(properties, item);
        }

        public virtual byte[] ExportCatalogItemsARToXlsx(IEnumerable<UCatalogExport> item)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<UCatalogExport>("id", p => p.ItemId),
                new PropertyByName<UCatalogExport>("title", p => p.ProductNameAr),
                new PropertyByName<UCatalogExport>("description", p => p.DescriptionAr),
                new PropertyByName<UCatalogExport>("availability", p => p.Availability),
                new PropertyByName<UCatalogExport>("condition", p => p.Condition),
                new PropertyByName<UCatalogExport>("price", p => p.Price),
                new PropertyByName<UCatalogExport>("link", p => p.Link),
                new PropertyByName<UCatalogExport>("image_link", p => p.ImageUrl),
                new PropertyByName<UCatalogExport>("brand", p => p.Brand),
                new PropertyByName<UCatalogExport>("collection", p => p.CategoryNameAr),
                new PropertyByName<UCatalogExport>("collection_name", p => p.CategoryNameAr)
            };

            return ExportToXlsx(properties, item);
        }
        public virtual byte[] ExportTemplateAnalyticReportToCsv(IEnumerable<UTemplateAnalyticsSummary> item)
        {
            var properties = new[]
{
                new PropertyByName<UTemplateAnalyticsSummary>("RecordDate", p => p.RecordDate),
                new PropertyByName<UTemplateAnalyticsSummary>("ClientId", p => p.ClientId),
                new PropertyByName<UTemplateAnalyticsSummary>("SenderId", p => p.SenderId),
                new PropertyByName<UTemplateAnalyticsSummary>("TemplateId", p => p.TemplateId),
                new PropertyByName<UTemplateAnalyticsSummary>("SentCount", p => p.SentCount),
                new PropertyByName<UTemplateAnalyticsSummary>("DeliveredCount", p => p.DeliveredCount),
                new PropertyByName<UTemplateAnalyticsSummary>("ReadCount", p => p.ReadCount),
                new PropertyByName<UTemplateAnalyticsSummary>("FailedCount", p => p.FailedCount),
                new PropertyByName<UTemplateAnalyticsSummary>("Amount_Spent", p => p.Amount_Spent),
                new PropertyByName<UTemplateAnalyticsSummary>("CostPerDelivered", p => p.CostPerDelivered),
                new PropertyByName<UTemplateAnalyticsSummary>("CostPerUrlButtonClick", p => p.CostPerUrlButtonClick),
                new PropertyByName<UTemplateAnalyticsSummary>("Details", p => p.Details)
            };
            return ExportToXlsx(properties, item);
        }
        public virtual byte[] ExportTemplateAnalyticDetailsReportToCsv(IEnumerable<UTemplateAnalyticsDetailsList> item)
        {
            var properties = new[]
{
                new PropertyByName<UTemplateAnalyticsDetailsList>("ClientId", p => p.ClientId),
                new PropertyByName<UTemplateAnalyticsDetailsList>("SenderId", p => p.SenderId),
                new PropertyByName<UTemplateAnalyticsDetailsList>("TemplateId", p => p.TemplateId),
                new PropertyByName<UTemplateAnalyticsDetailsList>("SentCount", p => p.SentCount),
                new PropertyByName<UTemplateAnalyticsDetailsList>("DeliveredCount", p => p.DeliveredCount),
                new PropertyByName<UTemplateAnalyticsDetailsList>("ReadCount", p => p.ReadCount),
                new PropertyByName<UTemplateAnalyticsDetailsList>("FailedCount", p => p.FailedCount),
                new PropertyByName<UTemplateAnalyticsDetailsList>("ButtonDetails", p => p.ButtonDetails)
            };
            return ExportToXlsx(properties, item);
        }

        public virtual byte[] ExportCatalogItemsENToCsv(IEnumerable<UCatalogExport> item)
        {
            var csvBytes = ConvertToCsvBytes(item, new UCatalogExportMapEN());
            return csvBytes;
        }

        public virtual byte[] ExportCatalogItemsARToCsv(IEnumerable<UCatalogExport> item)
        {
            var csvBytes = ConvertToCsvBytes(item, new UCatalogExportMapAR());
            return csvBytes;
        }
    }
}
