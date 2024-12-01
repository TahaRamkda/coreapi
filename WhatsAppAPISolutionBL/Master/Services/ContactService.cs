using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WhatsAppAPISolutionBL.Master.Helper;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class ContactService : IContactService
    {
        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;

        public ContactService(WhatsAppSolutionContext dbContext, WhatsAppSolutionContext2 dbContext2)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
        }

        public async Task<List<UContact>> GetContactListAsync(int ClientId, string SearchStr = "")
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @ClientId={1}, @SearchStr='{2}'", (int)CrudEnum.List, ClientId, SearchStr);
            var response = await _dbContext2.Contacts.FromSqlRaw(query).ToListAsync();

            return response;
        }
        public async Task<UResponse> AddContactAsync(ContactDto contact)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @GroupId={1}, @FirstName='{2}', @LastName='{3}', @PhoneNumber='{4}', @EmailAddress='{5}', @AreaName='{6}', @ActionBy={7}", (int)CrudEnum.Add, contact.GroupId, contact.FirstName, contact.LastName, contact.PhoneNumber, contact.EmailAddress, contact.AreaName, contact.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

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
    })
    .ToList();

            var response = await InsertBulkContactAsync(contact);
            return response;
        }

        public async Task<UResponse> UpdateContactAsync(ContactDto contact)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @ContactId={1}, @GroupId={2}, @FirstName='{3}', @LastName='{4}', @PhoneNumber='{5}', @EmailAddress='{6}', @AreaName='{7}', @ActionBy={8}", (int)CrudEnum.Update, contact.ContactId, contact.GroupId, contact.FirstName, contact.LastName, contact.PhoneNumber, contact.EmailAddress, contact.AreaName, contact.ActionBy);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> DeleteContactAsync(int ContactId)
        {
            var query = string.Format(@"exec usp_Contacts_Ops @ActionId={0}, @ContactId={1}", (int)CrudEnum.Delete, ContactId);
            var response = await _dbContext2.Response.FromSqlRaw(query).ToListAsync();

            return response[0];
        }
        public async Task<UResponse> InsertBulkContactAsync(BulkContactDto contact)
        {
            var contactJson = JsonSerializer.Serialize(contact.ContactsInfo);
            var response = await _dbContext2.Response.FromSqlInterpolated($"exec usp_Contacts_Ops  @ActionId={(int)CrudEnum.BulkContact}, @GroupId={contact.GroupId}, @ClientId={contact.ClientId}, @BulkContact={contactJson}, @ActionBy={contact.ActionBy}").ToListAsync();

            return response[0];
        }
    }
}
