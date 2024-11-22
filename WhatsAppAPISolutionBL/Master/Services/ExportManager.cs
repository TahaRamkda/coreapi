using OfficeOpenXml;
using OfficeOpenXml.Style;
using WhatsAppAPISolutionBL.Master.Help;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.Extensions;
using WhatsAppAPISolutionDL.Models;

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
    }
}
