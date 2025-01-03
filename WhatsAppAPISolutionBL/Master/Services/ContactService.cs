using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Contact;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using WhatsAppAPISolutionDL.UserModels.Contact;
using WhatsAppAPISolutionDL.UserModels.Entity;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ContactService : IContactService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly IImportManager _importManager;

        public ContactService(WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            IImportManager importManager)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _importManager = importManager;
        }

        public async Task<List<UContact>> GetContactListAsync(int ClientId, int GroupId = 0, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Contacts.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @GroupId={GroupId}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddContactAsync(ContactDto contact)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={contact.ClientId}, @GroupId={contact.GroupId}, @FirstName={contact.FirstName}, @LastName={contact.LastName}, @PhoneNumber={contact.PhoneNumber}, @EmailAddress={contact.EmailAddress}, @AreaName={contact.AreaName}, @ActionBy={contact.ActionBy}").ToListAsync();
            return response[0];
        }


        public async Task<UResponse> UpdateContactAsync(ContactDto contact)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Update}, @ContactId={contact.ContactId}, @GroupId={contact.GroupId}, @FirstName={contact.FirstName}, @LastName={contact.LastName}, @PhoneNumber={contact.PhoneNumber}, @EmailAddress={contact.EmailAddress}, @AreaName={contact.AreaName}, @ActionBy={contact.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> DeleteContactAsync(int ContactId)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Delete}, @ContactId={ContactId}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> ImportBulkContacts(ImportContactDto model)
        {
            var contacts = _importManager.ImportContactsFromXlsx(model.File.OpenReadStream());
            if (contacts == null || !contacts.Any())
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot import contacts, No contacts found"
                };
            }

            var contactsJson = JsonSerializer.Serialize(contacts);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops  @ActionId={(int)CrudEnum.BulkContact}, @ClientId={model.ClientId}, @BulkContact={contactsJson}, @ActionBy={model.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UContactDetail> GetContactByIdAsync(int clientId, int contactId)
        {
            var response = await _dbContext2.ContactDetails.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.GetById}, @ClientId={clientId},@ContactId={contactId}").ToListAsync();
            if (response == null || response.Count == 0)
                return null;
            return response[0];
        }
    }
}
