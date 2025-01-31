using OfficeOpenXml;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Agent;
using WhatsAppAPISolutionDL.Dto.Contact;

namespace WhatsAppAPISolutionBL.Master.Services
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        /// <summary>
        /// Import contacts from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual List<ContactInfo> ImportContactsFromXlsx(Stream stream)
        {
            var contacts = new List<ContactInfo>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assume data is in the first worksheet
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Assuming first row is the header
                {
                    var contact = new ContactInfo
                    {
                        GroupName = (worksheet.Cells[row, 1].Value?.ToString() ?? ""),
                        FirstName = (worksheet.Cells[row, 2].Value?.ToString() ?? ""),
                        LastName = (worksheet.Cells[row, 3].Value?.ToString() ?? ""),
                        PhoneNumber = (worksheet.Cells[row, 4].Value?.ToString() ?? ""),
                        EmailAddress = (worksheet.Cells[row, 5].Value?.ToString() ?? ""),
                        AreaName = (worksheet.Cells[row, 6].Value?.ToString() ?? ""),
                    };

                    contacts.Add(contact);
                }
            }

            return contacts;
        }

        /// <summary>
        /// Import Bulk Agent from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual List<AgentTimingInfo> ImportBulkAgentTimingsFromXlsx(Stream stream)
        {
            var agentTimings = new List<AgentTimingInfo>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assume data is in the first worksheet
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Assuming first row is the header
                {
                    var agentTiming = new AgentTimingInfo
                    {
                        AgentId = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                        AgentName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                        Shift = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                        StartTime = DateTime.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out DateTime startTime) ? startTime.TimeOfDay : TimeSpan.Zero,
                        EndTime = DateTime.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out DateTime endTime) ? endTime.TimeOfDay : TimeSpan.Zero,
                        Sunday = ConvertToBoolean(worksheet.Cells[row, 6].Value?.ToString()),
                        Monday = ConvertToBoolean(worksheet.Cells[row, 7].Value?.ToString()),
                        Tuesday = ConvertToBoolean(worksheet.Cells[row, 8].Value?.ToString()),
                        Wednesday = ConvertToBoolean(worksheet.Cells[row, 9].Value?.ToString()),
                        Thursday = ConvertToBoolean(worksheet.Cells[row, 10].Value?.ToString()),
                        Friday = ConvertToBoolean(worksheet.Cells[row, 11].Value?.ToString()),
                        Saturday = ConvertToBoolean(worksheet.Cells[row, 12].Value?.ToString()),
                    };

                    agentTimings.Add(agentTiming);
                }
            }

            return agentTimings;
        }
        private static bool ConvertToBoolean(string value)
        {
            return value == "1";
        }
    }
}
