using WhatsAppAPISolutionDL.Dto.Contact;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IImportManager
    {
        /// <summary>
        /// Import contacts from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        List<ContactInfo> ImportContactsFromXlsx(Stream stream);
    }
}
