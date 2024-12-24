using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

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

        public async Task<List<UContact>> GetContactListAsync(int ClientId, string SearchStr = "", int SortBy = 0, int PageNo = 0, int PageSize = int.MaxValue)
        {
            var response = await _dbContext2.Contacts.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.List}, @ClientId={ClientId}, @SearchStr={SearchStr}, @SortBy={SortBy}, @PageNo={PageNo}, @PageSize={PageSize}").ToListAsync();
            return response;
        }

        public async Task<UResponse> AddContactAsync(ContactDto contact)
        {
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops @ActionId={(int)CrudEnum.Add}, @ClientId={contact.ClientId}, @GroupId={contact.GroupId}, @FirstName={contact.FirstName}, @LastName={contact.LastName}, @PhoneNumber={contact.PhoneNumber}, @EmailAddress={contact.EmailAddress}, @AreaName={contact.AreaName}, @ActionBy={contact.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> AddBulkContactAsync(BulkContactDto contact)
        {
            if (contact == null) throw new ArgumentNullException(nameof(contact));

            if (contact.GroupId == 0)
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please insert group"
                };
            if (!contact.ContactsInfo.Any())
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please add atleast one contact"
                };
            if (!contact.ContactsInfo.Select(x => x.PhoneNumber).Any())
                return new UResponse()
                {
                    Status = 0,
                    Message = "Please add phone numbers"
                };

            contact.ContactsInfo = contact.ContactsInfo
                .Where(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .Select(x => new ContactInfo
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber.TrimPhoneNumbers(),
                    EmailAddress = x.EmailAddress,
                    AreaName = x.AreaName
                }).ToList();

            var response = await InsertBulkContactAsync(contact);
            return response;
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

        public async Task<UResponse> InsertBulkContactAsync(BulkContactDto contact)
        {
            var contactJson = JsonSerializer.Serialize(contact.ContactsInfo);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops  @ActionId={(int)CrudEnum.BulkContact}, @GroupId={contact.GroupId}, @ClientId={contact.ClientId}, @BulkContact={contactJson}, @ActionBy={contact.ActionBy}").ToListAsync();
            return response[0];
        }

        public async Task<UResponse> ImportBulkContacts(IFormFile file, int clientId)
        {
            var contacts = _importManager.ImportContactsFromXlsx(file.OpenReadStream());
            if (contacts == null || !contacts.Any())
            {
                return new UResponse
                {
                    Status = 0,
                    Message = "Cannot import contacts"
                };
            }

            return null;
        }
    }
}
