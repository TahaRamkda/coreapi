using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Interfaces
{
    public interface IContactService
    {
        Task<List<UContact>> GetContactListAsync(int ClientId, int GroupId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue);
        Task<UResponse> AddContactAsync(ContactDto contact);
        Task<UResponse> UpdateContactAsync(ContactDto contact);
        Task<UResponse> DeleteContactAsync(int ContactId);
        Task<UResponse> ImportBulkContacts(ImportContactDto model);
        Task<UContactDetail> GetContactByIdAsync(int clientId, int contactId);
    }
}
